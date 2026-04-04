using System.ComponentModel.DataAnnotations;

namespace BookingService.Application.DTOs;

public class CreateBookingRequest
{
    [Required]
    public Guid ResourceId { get; set; }

    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }
}

public class UpdateBookingRequest
{
    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }
}

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Status { get; set; } = string.Empty;
}
