namespace BookingService.Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Пусто = доступен всем авторизованным; иначе только пользователям с тем же AccessGroup.</summary>
    public string? RequiredAccessGroup { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
