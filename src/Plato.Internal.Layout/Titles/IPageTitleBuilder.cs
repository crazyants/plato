using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Titles
{
    public interface IPageTitleBuilder
    {

        void Clear();

        void AddSegment(IHtmlContent segment, int position = 0);

        IHtmlContent GenerateTitle(IHtmlContent separator);

    }
}
