using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);

        if (!await db.Users.AnyAsync(u => u.Role == UserRole.Admin, ct))
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                AccessGroup = null
            };
            await db.Users.AddAsync(admin, ct);
            await db.SaveChangesAsync(ct);
        }

        if (await db.Resources.AnyAsync(ct))
            return;

        var samples = new List<Resource>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Переговорная «Сириус»",
                Description = "До 8 человек, флипчарт, экран",
                IsActive = true,
                RequiredAccessGroup = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Переговорная «Вега»",
                Description = "Небольшая комната, до 4 человек",
                IsActive = true,
                RequiredAccessGroup = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Проектор (портативный)",
                Description = "Full HD, сумка и кабели в комплекте",
                IsActive = true,
                RequiredAccessGroup = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Зал для презентаций",
                Description = "До 35 мест; доступ только группе staff",
                IsActive = true,
                RequiredAccessGroup = "staff"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ноутбук в аренду",
                Description = "Windows, офис; выдача на сутки",
                IsActive = true,
                RequiredAccessGroup = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Место в open space",
                Description = "Фиксированное рабочее место на день",
                IsActive = true,
                RequiredAccessGroup = null
            }
        };

        await db.Resources.AddRangeAsync(samples, ct);
        await db.SaveChangesAsync(ct);
    }
}
