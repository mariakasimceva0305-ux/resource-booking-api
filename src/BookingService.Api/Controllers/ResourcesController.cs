using BookingService.Api.Extensions;
using BookingService.Application.Abstractions;
using BookingService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resources;

    public ResourcesController(IResourceService resources)
    {
        _resources = resources;
    }

    [HttpGet]
    public Task<IReadOnlyList<ResourceDto>> List(CancellationToken ct)
    {
        var isAdmin = User.IsAdmin();
        var group = User.GetAccessGroup();
        return _resources.ListVisibleAsync(isAdmin, group, ct);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public Task<ResourceDto> Create([FromBody] CreateResourceRequest request, CancellationToken ct) =>
        _resources.CreateAsync(request, ct);
}
