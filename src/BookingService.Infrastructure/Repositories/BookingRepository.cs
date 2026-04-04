using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _db;

    public BookingRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Booking booking, CancellationToken ct = default) =>
        await _db.Bookings.AddAsync(booking, ct);

    public void Update(Booking booking) => _db.Bookings.Update(booking);

    public Task<int> CountActiveOverlapsAsync(
        Guid resourceId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeBookingId,
        CancellationToken ct = default)
    {
        var q = _db.Bookings.AsNoTracking()
            .Where(b => b.ResourceId == resourceId && b.Status == BookingStatus.Active
                && b.StartUtc < endUtc && b.EndUtc > startUtc);
        if (excludeBookingId.HasValue)
            q = q.Where(b => b.Id != excludeBookingId.Value);
        return q.CountAsync(ct);
    }

    public Task<int> CountActiveForUserOnDayAsync(Guid userId, DateTime utcDay, CancellationToken ct = default)
    {
        var start = utcDay.Date;
        var end = start.AddDays(1);
        return _db.Bookings.AsNoTracking()
            .CountAsync(b => b.UserId == userId && b.Status == BookingStatus.Active
                && b.StartUtc >= start && b.StartUtc < end, ct);
    }

    public async Task<IReadOnlyList<Booking>> ListForUserAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Bookings.AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartUtc)
            .ToListAsync(ct);

    public Task<int> CountActiveAsync(CancellationToken ct = default) =>
        _db.Bookings.AsNoTracking().CountAsync(b => b.Status == BookingStatus.Active, ct);

    public Task<int> CountStartsOnUtcDayAsync(DateTime utcDay, CancellationToken ct = default)
    {
        var start = DateTime.SpecifyKind(utcDay.Date, DateTimeKind.Utc);
        var end = start.AddDays(1);
        return _db.Bookings.AsNoTracking()
            .CountAsync(b => b.Status == BookingStatus.Active && b.StartUtc >= start && b.StartUtc < end, ct);
    }
}
