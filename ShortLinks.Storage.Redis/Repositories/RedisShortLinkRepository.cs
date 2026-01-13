using ShortLinks.Abstractions.Interfaces.Repositories;
using ShortLinks.Abstractions.Models;
using ShortLinks.Storage.Redis.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace ShortLinks.Storage.Redis.Repositories
{
    public sealed class RedisShortLinkRepository : IShortLinkRepository
    {
        private readonly IDatabase _db;
        private readonly RedisShortLinksOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public RedisShortLinkRepository(IConnectionMultiplexer mux, RedisShortLinksOptions options)
        {
            _db = mux.GetDatabase();
            _options = options;
        }

        private string Key(string code) => $"{_options.KeyPrefix}{code}";

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => _db.KeyExistsAsync(Key(code));

        public async Task SaveAsync(ShortLink link, CancellationToken ct = default)
        {
            if (link is null) throw new ArgumentNullException(nameof(link));
            if (string.IsNullOrWhiteSpace(link.Code)) throw new ArgumentException("Code is required.", nameof(link));
            if (string.IsNullOrWhiteSpace(link.OriginalUrl)) throw new ArgumentException("OriginalUrl is required.", nameof(link));

            var key = Key(link.Code);

            var payload = Serialize(link);

            TimeSpan? ttl = null;
            if (_options.UseKeyTtlForExpiration && link.ExpireAtUtc.HasValue)
            {
                ttl = link.ExpireAtUtc.Value - DateTime.UtcNow;
                if (ttl <= TimeSpan.Zero)
                    throw new InvalidOperationException("ExpireAtUtc must be in the future.");
            }

            // Avoid overwrite (collision)
            var ok = await _db.StringSetAsync(
                key,
                payload,
                expiry: ttl,
                when: When.NotExists);

            if (!ok)
                throw new InvalidOperationException($"Code '{link.Code}' already exists.");
        }

        public async Task<ShortLink?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            var value = await _db.StringGetAsync(Key(code));
            if (value.IsNullOrEmpty)
                return null;

            try
            {
                return Deserialize(value!);
            }
            catch
            {
                // If payload got corrupted or schema changed, treat as not found
                return null;
            }
        }

        public async Task UpdateTrackingAsync(ShortLink link, CancellationToken ct = default)
        {
            if (link is null) throw new ArgumentNullException(nameof(link));

            var key = Key(link.Code);

            // Preserve TTL (important for expiring links)
            TimeSpan? ttl = null;
            if (_options.UseKeyTtlForExpiration)
            {
                ttl = await _db.KeyTimeToLiveAsync(key);
                // ttl can be null (no expiry) or -1 depending on redis version/scenario
            }

            var payload = Serialize(link);

            // Update only if key exists (avoid creating new keys by accident)
            var ok = await _db.StringSetAsync(
                key,
                payload,
                expiry: ttl,
                when: When.Exists);

            // If key doesn’t exist anymore (expired), just ignore
            _ = ok;
        }

        private RedisValue Serialize(ShortLink link)
        {
            if (!_options.UseJson)
                throw new NotSupportedException("Non-JSON serialization is not implemented.");

            return JsonSerializer.SerializeToUtf8Bytes(link, JsonOptions);
        }

        private ShortLink Deserialize(RedisValue value)
        {
            if (!_options.UseJson)
                throw new NotSupportedException("Non-JSON serialization is not implemented.");

            return JsonSerializer.Deserialize<ShortLink>((byte[])value!, JsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize ShortLink.");
        }
    }
}