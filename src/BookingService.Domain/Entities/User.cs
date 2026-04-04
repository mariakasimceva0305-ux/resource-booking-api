using BookingService.Domain.Enums;

namespace BookingService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? AccessGroup { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
