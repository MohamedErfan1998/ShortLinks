using Microsoft.AspNetCore.Mvc;
using ShortLinks.Abstractions.Interfaces.Services;

namespace ShortLinks.Api.Controllers;

[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IShortLinkService _service;

    public RedirectController(IShortLinkService service)
    {
        _service = service;
    }

    // GET /s/{code}
    [HttpGet("s/{code}")]
    public async Task<IActionResult> Go(string code, CancellationToken ct)
    {
        var resolved = await _service.ResolveAsync(code, ct);
        if (resolved is null)
            return NotFound();

        return Redirect(resolved.OriginalUrl);
    }
}
