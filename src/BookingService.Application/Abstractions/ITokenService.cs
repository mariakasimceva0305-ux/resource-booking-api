using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions;

public interface ITokenService
{
    string CreateToken(User user);
}
