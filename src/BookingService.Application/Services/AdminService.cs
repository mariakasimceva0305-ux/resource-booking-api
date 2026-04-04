using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;

namespace BookingService.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _users;
    private readonly IResourceRepository _resources;
    private readonly IBookingRepository _bookings;

    public AdminService(IUserRepository users, IResourceRepository resources, IBookingRepository bookings)
    {
        _users = users;
        _resources = resources;
        _bookings = bookings;
    }

    public async Task<AdminStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return new AdminStatsDto
        {
            TotalUsers = await _users.CountAsync(ct),
            TotalResources = await _resources.CountAsync(ct),
            ActiveBookings = await _bookings.CountActiveAsync(ct),
            BookingsToday = await _bookings.CountStartsOnUtcDayAsync(today, ct)
        };
    }
}
