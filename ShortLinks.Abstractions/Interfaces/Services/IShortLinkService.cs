using ShortLinks.Abstractions.Models;

namespace ShortLinks.Abstractions.Interfaces.Services
{
    public interface IShortLinkService
    {
        Task<CreateShortLinkResult> CreateAsync(CreateShortLinkRequest request, CancellationToken ct = default);
        Task<ResolveShortLinkResult?> ResolveAsync(string code, CancellationToken ct = default);
    }
}