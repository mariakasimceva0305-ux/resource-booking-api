using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using BookingService.Application.Exceptions;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;

namespace BookingService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, IUnitOfWork uow, ITokenService tokens)
    {
        _users = users;
        _uow = uow;
        _tokens = tokens;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new AppException(409, "Пользователь с таким email уже зарегистрирован.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User,
            AccessGroup = string.IsNullOrWhiteSpace(request.AccessGroup) ? null : request.AccessGroup.Trim()
        };
        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse
        {
            Token = _tokens.CreateToken(user),
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AppException(401, "Неверный email или пароль.");

        return new AuthResponse
        {
            Token = _tokens.CreateToken(user),
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
