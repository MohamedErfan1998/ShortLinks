using Microsoft.Extensions.Options;
using ShortLinks.Abstractions.Interfaces.Repositories;
using ShortLinks.Abstractions.Interfaces.Services;
using ShortLinks.Abstractions.Models;
using ShortLinks.Abstractions.Options;

namespace ShortLinks.Core.Services
{
    public sealed class ShortLinkService : IShortLinkService
    {
        private readonly IShortLinkRepository _repo;
        private readonly ICodeGenerator _generator;
        private readonly ShortLinksOptions _options;

        public ShortLinkService(
            IShortLinkRepository repo,
            ICodeGenerator generator,
            IOptions<ShortLinksOptions> options)
        {
            _repo = repo;
            _generator = generator;
            _options = options.Value;
        }

        public async Task<CreateShortLinkResult> CreateAsync(CreateShortLinkRequest request, CancellationToken ct = default)
        {
            UrlValidator.EnsureValidAbsoluteUrl(request.OriginalUrl);

            if (request.ExpireAtUtc is not null && request.ExpireAtUtc <= DateTime.UtcNow)
                throw new ArgumentException("ExpireAtUtc must be in the future.", nameof(request.ExpireAtUtc));

            if (!string.IsNullOrWhiteSpace(request.CustomCode))
                UrlValidator.EnsureValidCode(request.CustomCode);

            string code;

            if (!string.IsNullOrWhiteSpace(request.CustomCode))
            {
                code = request.CustomCode!;
                if (await _repo.CodeExistsAsync(code, ct))
                    throw new InvalidOperationException($"Code '{code}' already exists.");
            }
            else
            {
                // generate with collision checks
                code = await GenerateUniqueCodeAsync(ct);
            }

            var link = new ShortLink
            {
                Code = code,
                OriginalUrl = request.OriginalUrl,
                ExpireAtUtc = request.ExpireAtUtc,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repo.SaveAsync(link, ct);

            var shortUrl = $"{_options.BaseUrl.TrimEnd('/')}/{code}";
            return new CreateShortLinkResult(code, request.OriginalUrl, shortUrl);
        }

        public async Task<ResolveShortLinkResult?> ResolveAsync(string code, CancellationToken ct = default)
        {
            UrlValidator.EnsureValidCode(code);

            var link = await _repo.FindByCodeAsync(code, ct);
            if (link is null)
                return null;

            if (link.ExpireAtUtc is not null && link.ExpireAtUtc <= DateTime.UtcNow)
                return null;

            if (_options.TrackHits)
            {
                link.HitCount++;
                link.LastAccessedAtUtc = DateTime.UtcNow;
                await _repo.UpdateTrackingAsync(link, ct);
            }

            return new ResolveShortLinkResult(link.Code, link.OriginalUrl);
        }

        private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
        {
            for (int attempt = 0; attempt < _options.MaxCreateAttempts; attempt++)
            {
                var code = _generator.Generate(_options.DefaultCodeLength);
                if (!await _repo.CodeExistsAsync(code, ct))
                    return code;
            }

            throw new InvalidOperationException("Failed to generate a unique short code. Try increasing code length.");
        }
    }

}
