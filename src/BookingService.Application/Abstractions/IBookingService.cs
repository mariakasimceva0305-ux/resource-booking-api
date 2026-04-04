using BookingService.Application.DTOs;

namespace BookingService.Application.Abstractions;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(Guid userId, string? userAccessGroup, CreateBookingRequest request, CancellationToken ct = default);
    Task<BookingDto> UpdateAsync(Guid userId, bool isAdmin, Guid bookingId, UpdateBookingRequest request, CancellationToken ct = default);
    Task CancelAsync(Guid userId, bool isAdmin, Guid bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<BookingDto>> ListMineAsync(Guid userId, CancellationToken ct = default);
}
