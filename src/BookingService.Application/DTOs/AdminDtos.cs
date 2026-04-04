namespace BookingService.Application.DTOs;

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalResources { get; set; }
    public int ActiveBookings { get; set; }
    public int BookingsToday { get; set; }
}
