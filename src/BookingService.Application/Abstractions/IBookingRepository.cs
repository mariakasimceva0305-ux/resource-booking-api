using BookingService.Domain.Entities;
using BookingService.Domain.Enums;

namespace BookingService.Application.Abstractions;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    void Update(Booking booking);
    Task<int> CountActiveOverlapsAsync(Guid resourceId, DateTime startUtc, DateTime endUtc, Guid? excludeBookingId, CancellationToken ct = default);
    Task<int> CountActiveForUserOnDayAsync(Guid userId, DateTime dayUtc, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> ListForUserAsync(Guid userId, CancellationToken ct = default);
    Task<int> CountActiveAsync(CancellationToken ct = default);
    /// <summary>Активные брони, у которых дата начала (UTC) совпадает с переданным календарным днём.</summary>
    Task<int> CountStartsOnUtcDayAsync(DateTime utcDay, CancellationToken ct = default);
}
