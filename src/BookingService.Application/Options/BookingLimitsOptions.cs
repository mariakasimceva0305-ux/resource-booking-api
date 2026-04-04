namespace BookingService.Application.Options;

public class BookingLimitsOptions
{
    public const string SectionName = "BookingLimits";
    public int MaxBookingsPerDay { get; set; } = 5;
}
