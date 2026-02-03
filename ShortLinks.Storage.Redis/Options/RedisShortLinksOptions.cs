namespace ShortLinks.Storage.Redis.Options
{
    public sealed class RedisShortLinksOptions
    {
        /// <summary>
        /// Key prefix used in Redis.
        /// Example: "shortlinks:" => key becomes "shortlinks:Ab12Cd"
        /// </summary>
        public string KeyPrefix { get; set; } = "shortlinks:";

        /// <summary>
        /// If true, uses Redis key TTL based on ExpireAtUtc.
        /// </summary>
        public bool UseKeyTtlForExpiration { get; set; } = true;

        /// <summary>
        /// If true, store payload as JSON (default). Keep true unless you have special needs.
        /// </summary>
        public bool UseJson { get; set; } = true;
    }
}