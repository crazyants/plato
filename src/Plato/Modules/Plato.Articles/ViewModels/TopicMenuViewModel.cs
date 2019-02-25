using System.Collections.Generic;
using Plato.Entities.Models;

namespace Plato.Articles.ViewModels
{
    public class TopicMenuViewModel
    {

        public Entity Entity { get; set; }

        public IEnumerable<string> Views { get; set; }

    }
}
