namespace ShortLinks.Abstractions.Models
{
    public sealed record CreateShortLinkRequest(string OriginalUrl, DateTime? ExpireAtUtc = null, string? CustomCode = null);
}