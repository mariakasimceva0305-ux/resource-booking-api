using BookingService.Application.DTOs;

namespace BookingService.Application.Abstractions;

public interface IResourceService
{
    Task<ResourceDto> CreateAsync(CreateResourceRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ResourceDto>> ListVisibleAsync(bool isAdmin, string? userAccessGroup, CancellationToken ct = default);
}
