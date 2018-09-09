using System;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class ComposedLocaleResource
    {

        public LocaleResource LocaleResource { get; set; }

        public object Model { get; set; }

        public Type Type { get; set; }

        public void Configure<TModel>(Func<TModel, LocaleValues<TModel>> configure) where TModel : class
        {
            this.Model = configure(default(TModel));
            this.Type = typeof(TModel);
        }

    }

}
