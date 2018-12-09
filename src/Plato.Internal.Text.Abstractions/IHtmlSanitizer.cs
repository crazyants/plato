namespace Plato.Internal.Text.Abstractions
{
    public interface IHtmlSanitizer
    {
        string SanitizeHtml(string html);

        string SanitizeHtml(string html, string[] excludeTags);
    }

}
