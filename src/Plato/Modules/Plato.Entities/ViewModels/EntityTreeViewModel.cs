using System.Collections.Generic;
using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityTreeViewModel
    {
        
        public IList<Selection<Entity>> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

    }

    public class Selection<TModel> where TModel : class, IEntity
    {

        public bool IsSelected { get; set; }

        public TModel Value { get; set; }

    }

}
