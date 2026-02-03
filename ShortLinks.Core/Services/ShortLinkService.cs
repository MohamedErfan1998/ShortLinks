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

            var maxUses = request.OneTime ? 1 : request.MaxUses;

            if (maxUses is not null && maxUses <= 0)
                throw new ArgumentException("MaxUses must be greater than zero.", nameof(request.MaxUses));

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
                CreatedAtUtc = DateTime.UtcNow,
                MaxUses = maxUses,
                UsedCount = 0
            };

            await _repo.SaveAsync(link, ct);

            var shortUrl = $"{_options.BaseUrl.TrimEnd('/')}/{code}";
            return new CreateShortLinkResult(code, request.OriginalUrl, shortUrl);
        }

        public async Task<ResolveShortLinkResult?> ResolveAsync(string code, CancellationToken ct = default)
        {
            UrlValidator.EnsureValidCode(code);

            var link = await _repo.ConsumeAsync(code, _options.TrackHits, ct);
            if (link is null)
                return null;

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
