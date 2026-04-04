namespace BookingService.Application.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "BookingService";
    public string Audience { get; set; } = "BookingService";
    public int ExpireMinutes { get; set; } = 60;
}
