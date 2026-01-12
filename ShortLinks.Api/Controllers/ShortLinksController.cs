using Microsoft.AspNetCore.Mvc;
using ShortLinks.Abstractions.Interfaces.Services;
using ShortLinks.Abstractions.Models;

namespace ShortLinks.Api.Controllers;

[ApiController]
[Route("api/shortlinks")]
public class ShortLinksController : ControllerBase
{
    private readonly IShortLinkService _service;

    public ShortLinksController(IShortLinkService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CreateShortLinkResult>> Create([FromBody] CreateShortLinkRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<ResolveShortLinkResult>> Resolve(string code, CancellationToken ct)
    {
        var result = await _service.ResolveAsync(code, ct);
        return result is null ? NotFound() : Ok(result);
    }
}