using Microsoft.EntityFrameworkCore;
using ShortLinks.Abstractions.Interfaces.Repositories;
using ShortLinks.Abstractions.Models;

namespace ShortLinks.Storage.EFCore.Repositories
{
    public sealed class EfCoreShortLinkRepository : IShortLinkRepository
    {
        private readonly ShortLinksDbContext _db;

        public EfCoreShortLinkRepository(ShortLinksDbContext db) => _db = db;

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => _db.ShortLinks.AsNoTracking().AnyAsync(x => x.Code == code, ct);

        public async Task SaveAsync(ShortLink link, CancellationToken ct = default)
        {
            _db.ShortLinks.Add(link);
            await _db.SaveChangesAsync(ct);
        }

        public Task<ShortLink?> FindByCodeAsync(string code, CancellationToken ct = default)
            => _db.ShortLinks.FirstOrDefaultAsync(x => x.Code == code, ct);

        public async Task UpdateTrackingAsync(ShortLink link, CancellationToken ct = default)
        {
            // link is tracked if loaded via FindByCodeAsync; but safe to call Update.
            _db.ShortLinks.Update(link);
            await _db.SaveChangesAsync(ct);
        }
    }
}