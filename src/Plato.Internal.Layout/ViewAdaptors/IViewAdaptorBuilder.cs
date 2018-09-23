using System;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.ViewAdaptors
{
    public interface IViewAdaptorBuilder
    {

        string ViewName { get; }

        IViewAdaptorBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);

        IViewAdaptorBuilder AdaptView(string viewName);

        IViewAdaptorBuilder AdaptView(string[] viewNames);

        IViewAdaptorBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class;

    }

}
