namespace ShortLinks.Abstractions.Models
{
    public sealed record ResolveShortLinkResult(string Code, string OriginalUrl);
}
