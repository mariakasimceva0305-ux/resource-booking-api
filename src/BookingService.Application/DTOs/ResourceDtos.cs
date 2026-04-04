using System.ComponentModel.DataAnnotations;

namespace BookingService.Application.DTOs;

public class CreateResourceRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? RequiredAccessGroup { get; set; }
}

public class ResourceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? RequiredAccessGroup { get; set; }
}
