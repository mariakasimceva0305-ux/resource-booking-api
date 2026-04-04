using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public Task<AuthResponse> Register([FromBody] RegisterRequest request, CancellationToken ct) =>
        _auth.RegisterAsync(request, ct);

    [HttpPost("login")]
    [AllowAnonymous]
    public Task<AuthResponse> Login([FromBody] LoginRequest request, CancellationToken ct) =>
        _auth.LoginAsync(request, ct);
}
