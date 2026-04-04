using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions;

public interface IResourceRepository
{
    Task<Resource?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Resource>> ListActiveAsync(CancellationToken ct = default);
    Task AddAsync(Resource resource, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}
