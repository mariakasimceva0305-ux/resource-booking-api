using BookingService.Application.DTOs;

namespace BookingService.Application.Abstractions;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync(CancellationToken ct = default);
}
