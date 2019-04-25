using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Titles
{
    public interface IPageTitleBuilder
    {

        void Clear();

        void AddSegment(IHtmlContent segment, int order = 0);

        IHtmlContent GenerateTitle(IHtmlContent separator);

    }
}
