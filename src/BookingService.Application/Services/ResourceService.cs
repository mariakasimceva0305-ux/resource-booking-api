using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using BookingService.Domain.Entities;

namespace BookingService.Application.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resources;
    private readonly IUnitOfWork _uow;

    public ResourceService(IResourceRepository resources, IUnitOfWork uow)
    {
        _resources = resources;
        _uow = uow;
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceRequest request, CancellationToken ct = default)
    {
        var entity = new Resource
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = true,
            RequiredAccessGroup = string.IsNullOrWhiteSpace(request.RequiredAccessGroup)
                ? null
                : request.RequiredAccessGroup.Trim()
        };
        await _resources.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task<IReadOnlyList<ResourceDto>> ListVisibleAsync(bool isAdmin, string? userAccessGroup, CancellationToken ct = default)
    {
        var all = await _resources.ListActiveAsync(ct);
        if (isAdmin)
            return all.Select(Map).ToList();

        return all
            .Where(r => string.IsNullOrEmpty(r.RequiredAccessGroup)
                        || (!string.IsNullOrEmpty(userAccessGroup)
                            && string.Equals(r.RequiredAccessGroup, userAccessGroup, StringComparison.OrdinalIgnoreCase)))
            .Select(Map)
            .ToList();
    }

    private static ResourceDto Map(Resource r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Description = r.Description,
        IsActive = r.IsActive,
        RequiredAccessGroup = r.RequiredAccessGroup
    };
}
