using ShortLinks.Abstractions.Interfaces.Services;

namespace ShortLinks.Core.Services
{
    public sealed class Base62CodeGenerator : ICodeGenerator
    {
        private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Generate(int length)
        {
            if (length < 4 || length > 32)
                throw new ArgumentOutOfRangeException(nameof(length), "length must be between 4 and 32.");

            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = Chars[Random.Shared.Next(Chars.Length)];

            return new string(chars);
        }
    }
}