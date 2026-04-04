using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using BookingService.Application.Exceptions;
using BookingService.Application.Options;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using Microsoft.Extensions.Options;

namespace BookingService.Application.Services;

public class BookingAppService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IResourceRepository _resources;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly BookingLimitsOptions _limits;

    public BookingAppService(
        IBookingRepository bookings,
        IResourceRepository resources,
        IUserRepository users,
        IUnitOfWork uow,
        IOptions<BookingLimitsOptions> limits)
    {
        _bookings = bookings;
        _resources = resources;
        _users = users;
        _uow = uow;
        _limits = limits.Value;
    }

    public async Task<BookingDto> CreateAsync(Guid userId, string? userAccessGroup, CreateBookingRequest request, CancellationToken ct = default)
    {
        if (request.EndUtc <= request.StartUtc)
            throw new AppException(400, "Время окончания должно быть позже начала.");

        var resource = await _resources.GetByIdAsync(request.ResourceId, ct)
            ?? throw new AppException(404, "Ресурс не найден.");

        if (!resource.IsActive)
            throw new AppException(400, "Ресурс недоступен для бронирования.");

        if (!string.IsNullOrEmpty(resource.RequiredAccessGroup))
        {
            if (string.IsNullOrEmpty(userAccessGroup)
                || !string.Equals(resource.RequiredAccessGroup, userAccessGroup, StringComparison.OrdinalIgnoreCase))
                throw new AppException(403, "Нет доступа к этому ресурсу.");
        }

        var startUtc = NormalizeUtc(request.StartUtc);
        var endUtc = NormalizeUtc(request.EndUtc);

        var day = startUtc.Date;
        var countDay = await _bookings.CountActiveForUserOnDayAsync(userId, day, ct);
        if (countDay >= _limits.MaxBookingsPerDay)
            throw new AppException(400, $"Превышен лимит активных бронирований в день ({_limits.MaxBookingsPerDay}).");

        var overlaps = await _bookings.CountActiveOverlapsAsync(
            request.ResourceId, startUtc, endUtc, excludeBookingId: null, ct);
        if (overlaps > 0)
            throw new AppException(409, "Интервал пересекается с существующей бронью.");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ResourceId = resource.Id,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Status = BookingStatus.Active
        };
        await _bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);

        return await MapAsync(booking.Id, ct) ?? throw new AppException(500, "Не удалось прочитать созданную бронь.");
    }

    public async Task<BookingDto> UpdateAsync(Guid userId, bool isAdmin, Guid bookingId, UpdateBookingRequest request, CancellationToken ct = default)
    {
        if (request.EndUtc <= request.StartUtc)
            throw new AppException(400, "Время окончания должно быть позже начала.");

        var booking = await _bookings.GetByIdAsync(bookingId, ct)
            ?? throw new AppException(404, "Бронирование не найдено.");

        if (booking.Status != BookingStatus.Active)
            throw new AppException(400, "Нельзя изменить отменённое бронирование.");

        if (!isAdmin && booking.UserId != userId)
            throw new AppException(403, "Нет прав на изменение этой брони.");

        var startUtc = NormalizeUtc(request.StartUtc);
        var endUtc = NormalizeUtc(request.EndUtc);

        var overlaps = await _bookings.CountActiveOverlapsAsync(
            booking.ResourceId, startUtc, endUtc, excludeBookingId: booking.Id, ct);
        if (overlaps > 0)
            throw new AppException(409, "Новый интервал пересекается с другой бронью.");

        booking.StartUtc = startUtc;
        booking.EndUtc = endUtc;
        _bookings.Update(booking);
        await _uow.SaveChangesAsync(ct);

        return await MapAsync(booking.Id, ct) ?? throw new AppException(500, "Ошибка чтения брони.");
    }

    public async Task CancelAsync(Guid userId, bool isAdmin, Guid bookingId, CancellationToken ct = default)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, ct)
            ?? throw new AppException(404, "Бронирование не найдено.");

        if (!isAdmin && booking.UserId != userId)
            throw new AppException(403, "Нет прав на отмену этой брони.");

        if (booking.Status != BookingStatus.Active)
            return;

        booking.Status = BookingStatus.Cancelled;
        _bookings.Update(booking);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<BookingDto>> ListMineAsync(Guid userId, CancellationToken ct = default)
    {
        var list = await _bookings.ListForUserAsync(userId, ct);
        var result = new List<BookingDto>();
        foreach (var b in list)
            result.Add(await MapEntityAsync(b, ct));
        return result;
    }

    private static DateTime NormalizeUtc(DateTime dt) =>
        dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
        };

    private async Task<BookingDto?> MapAsync(Guid id, CancellationToken ct)
    {
        var b = await _bookings.GetByIdAsync(id, ct);
        return b is null ? null : await MapEntityAsync(b, ct);
    }

    private async Task<BookingDto> MapEntityAsync(Booking b, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(b.UserId, ct);
        var resource = await _resources.GetByIdAsync(b.ResourceId, ct);
        return new BookingDto
        {
            Id = b.Id,
            UserId = b.UserId,
            UserEmail = user?.Email ?? "",
            ResourceId = b.ResourceId,
            ResourceName = resource?.Name ?? "",
            StartUtc = b.StartUtc,
            EndUtc = b.EndUtc,
            Status = b.Status.ToString()
        };
    }
}
