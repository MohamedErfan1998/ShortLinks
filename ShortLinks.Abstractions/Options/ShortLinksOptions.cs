namespace ShortLinks.Abstractions.Options
{
    public sealed class ShortLinksOptions
    {
        /// <summary>
        /// Base url returned to the caller. Example: https://sbc.ly/s
        /// </summary>
        public string BaseUrl { get; set; } = "https://localhost:5001/s";

        /// <summary>
        /// Default code length when CustomCode is not provided.
        /// </summary>
        public int DefaultCodeLength { get; set; } = 8;

        /// <summary>
        /// Max attempts to avoid collision.
        /// </summary>
        public int MaxCreateAttempts { get; set; } = 10;

        /// <summary>
        /// If true, increments hit count and stores LastAccessedAt.
        /// </summary>
        public bool TrackHits { get; set; } = true;
    }

}
