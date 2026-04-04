namespace BookingService.Domain;

public static class BookingOverlap
{
    /// <summary>Пересечение полуоткрытых интервалов [start, end): пересекаются, если start1 &lt; end2 и start2 &lt; end1.</summary>
    public static bool IntervalsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2) =>
        start1 < end2 && start2 < end1;
}
