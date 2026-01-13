using ShortLinks.Abstractions.Models;

namespace ShortLinks.Abstractions.Interfaces.Repositories
{
    public interface IShortLinkRepository
    {
        Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
        Task SaveAsync(ShortLink link, CancellationToken ct = default);

        Task<ShortLink?> FindByCodeAsync(string code, CancellationToken ct = default);

        /// <summary>
        /// Persist tracking changes (hit count, last accessed). No-op for stores that don't track.
        /// </summary>
        Task UpdateTrackingAsync(ShortLink link, CancellationToken ct = default);

        // atomic consume
        Task<ShortLink?> ConsumeAsync(string code, bool trackHits, CancellationToken ct = default);
    }
}