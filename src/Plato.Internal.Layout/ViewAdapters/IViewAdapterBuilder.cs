using System;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.ViewAdapters
{
    public interface IViewAdapterBuilder
    {

        string ViewName { get; }

        IViewAdapterBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);

        IViewAdapterBuilder AdaptView(string viewName);

        IViewAdapterBuilder AdaptView(string[] viewNames);

        IViewAdapterBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class;

    }

}
