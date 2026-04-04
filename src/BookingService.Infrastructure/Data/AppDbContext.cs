using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(320);
        });

        modelBuilder.Entity<Resource>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.ResourceId, x.Status, x.StartUtc, x.EndUtc });
            e.HasOne(x => x.User).WithMany(u => u.Bookings).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Resource).WithMany(r => r.Bookings).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
