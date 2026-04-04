using BookingService.Domain;

namespace BookingService.Tests;

public class BookingOverlapTests
{
    [Fact]
    public void Overlapping_intervals_true()
    {
        var a1 = new DateTime(2026, 4, 4, 10, 0, 0, DateTimeKind.Utc);
        var a2 = new DateTime(2026, 4, 4, 11, 0, 0, DateTimeKind.Utc);
        var b1 = new DateTime(2026, 4, 4, 10, 30, 0, DateTimeKind.Utc);
        var b2 = new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Utc);
        Assert.True(BookingOverlap.IntervalsOverlap(a1, a2, b1, b2));
    }

    [Fact]
    public void Touching_at_boundary_no_overlap_for_half_open_convention()
    {
        var a1 = new DateTime(2026, 4, 4, 10, 0, 0, DateTimeKind.Utc);
        var a2 = new DateTime(2026, 4, 4, 11, 0, 0, DateTimeKind.Utc);
        var b1 = a2;
        var b2 = new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Utc);
        Assert.False(BookingOverlap.IntervalsOverlap(a1, a2, b1, b2));
    }
}
