namespace ShortLinks.Abstractions.Models
{
    public sealed class ShortLink
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = default!;
        public string OriginalUrl { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ExpireAtUtc { get; set; }

        public int HitCount { get; set; }
        public DateTime? LastAccessedAtUtc { get; set; }

        public int? MaxUses { get; set; }     // null = unlimited
        public int UsedCount { get; set; }    // increments on resolve
    }
}