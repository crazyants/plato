using System.Collections.Generic;
using Plato.Articles.Models;
using Plato.Internal.Models;

namespace Plato.Articles.ViewModels
{
    public class ArticleListItemViewModel
    {
        
        public Article Article { get; set; }
        
        public ILabelBase Channel { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }
       
    }
}
