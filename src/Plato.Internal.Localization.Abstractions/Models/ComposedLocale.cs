using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class ComposedLocaleDescriptor
    {

        public LocaleDescriptor Descriptor { get; set; }

        public IEnumerable<ComposedLocaleResource> Resources { get; set; }

    }

    public class ComposedLocaleResource
    {

        public LocaleResource LocaleResource { get; set; }

        public object Model { get; set; }

        public Type Type { get; set; }

        public void Compose<TModel>(Func<TModel, TModel> configure) where TModel : class
        {
            this.Model = configure(ActivateInstanceOf<TModel>.Instance());
            this.Type = typeof(TModel);
        }

    }


}
