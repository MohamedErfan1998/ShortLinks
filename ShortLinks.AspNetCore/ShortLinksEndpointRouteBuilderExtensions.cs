using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ShortLinks.Abstractions.Interfaces.Services;

namespace ShortLinks.AspNetCore
{

    public static class ShortLinksEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps GET /s/{code} that resolves & redirects.
        /// </summary>
        public static IEndpointRouteBuilder MapShortLinksRedirect(this IEndpointRouteBuilder endpoints, string pattern = "/s/{code}")
        {
            endpoints.MapGet(pattern, async (string code, IShortLinkService service, CancellationToken ct) =>
            {
                var resolved = await service.ResolveAsync(code, ct);
                if (resolved is null) return Results.NotFound();

                return Results.Redirect(resolved.OriginalUrl, permanent: false);
            });
            return endpoints;
        }
    }
}