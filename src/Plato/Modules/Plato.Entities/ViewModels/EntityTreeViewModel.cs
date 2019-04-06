using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityTreeViewModel
    {
        
        public IList<Selection<IEntity>> SelectedEntities { get; set; }

        public IEnumerable<IEntity> SelectedParents { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

    }

    public class Selection<TModel> where TModel : class, IEntity
    {

        public bool IsSelected { get; set; }

        public TModel Value { get; set; }

    }

}
