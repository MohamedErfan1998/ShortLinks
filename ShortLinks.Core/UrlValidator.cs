namespace ShortLinks.Core
{
    internal static class UrlValidator
    {
        public static void EnsureValidAbsoluteUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("OriginalUrl is required.", nameof(url));

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                throw new ArgumentException("OriginalUrl must be an absolute URL.", nameof(url));

            // Allow http/https only (common for shorteners)
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Only http/https URLs are allowed.", nameof(url));
        }

        public static void EnsureValidCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required.", nameof(code));

            // Base62-like constraint (optional but recommended)
            foreach (var ch in code)
            {
                var ok =
                    (ch >= '0' && ch <= '9') ||
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= 'A' && ch <= 'Z');

                if (!ok)
                    throw new ArgumentException("Code must be alphanumeric (A-Z, a-z, 0-9).", nameof(code));
            }

            if (code.Length < 4 || code.Length > 32)
                throw new ArgumentException("Code length must be between 4 and 32.", nameof(code));
        }
    }
}