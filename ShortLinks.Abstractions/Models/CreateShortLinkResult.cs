namespace ShortLinks.Abstractions.Models
{
    public sealed record CreateShortLinkResult(string Code, string OriginalUrl, string ShortUrl);

}