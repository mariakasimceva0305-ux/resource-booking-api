using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly AppDbContext _db;

    public ResourceRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Resource?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Resources.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Resource>> ListActiveAsync(CancellationToken ct = default) =>
        await _db.Resources.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task AddAsync(Resource resource, CancellationToken ct = default) =>
        await _db.Resources.AddAsync(resource, ct);

    public Task<int> CountAsync(CancellationToken ct = default) =>
        _db.Resources.CountAsync(ct);
}
