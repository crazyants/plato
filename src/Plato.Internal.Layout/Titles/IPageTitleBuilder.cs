using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Internal.Layout.Titles
{
    public interface IPageTitleBuilder
    {

        IPageTitleBuilder Clear();

        IPageTitleBuilder AddSegment(LocalizedString segment, int order = 0);
        
        IHtmlContent GenerateTitle(IHtmlContent separator);


    }
}
