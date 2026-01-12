namespace ShortLinks.Abstractions.Interfaces.Services
{
    public interface ICodeGenerator
    {
        string Generate(int length = 8);
    }
}