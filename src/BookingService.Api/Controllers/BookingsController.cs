using BookingService.Api.Extensions;
using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using BookingService.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;

    public BookingsController(IBookingService bookings)
    {
        _bookings = bookings;
    }

    [HttpGet("mine")]
    public async Task<IReadOnlyList<BookingDto>> Mine(CancellationToken ct)
    {
        var uid = User.GetUserId() ?? throw new AppException(401, "Не удалось определить пользователя.");
        return await _bookings.ListMineAsync(uid.Value, ct);
    }

    [HttpPost]
    public async Task<BookingDto> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var uid = User.GetUserId() ?? throw new AppException(401, "Не удалось определить пользователя.");
        return await _bookings.CreateAsync(uid.Value, User.GetAccessGroup(), request, ct);
    }

    [HttpPut("{id:guid}")]
    public async Task<BookingDto> Update(Guid id, [FromBody] UpdateBookingRequest request, CancellationToken ct)
    {
        var uid = User.GetUserId() ?? throw new AppException(401, "Не удалось определить пользователя.");
        return await _bookings.UpdateAsync(uid.Value, User.IsAdmin(), id, request, ct);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var uid = User.GetUserId() ?? throw new AppException(401, "Не удалось определить пользователя.");
        await _bookings.CancelAsync(uid.Value, User.IsAdmin(), id, ct);
        return NoContent();
    }
}
