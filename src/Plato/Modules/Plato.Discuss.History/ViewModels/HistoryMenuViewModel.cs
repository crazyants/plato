using Plato.Entities.Models;

namespace Plato.Discuss.History.ViewModels
{
    public class HistoryMenuViewModel
    {

        public IEntity Topic { get; set; }

        public IEntityReply Reply { get; set; }


    }
}
