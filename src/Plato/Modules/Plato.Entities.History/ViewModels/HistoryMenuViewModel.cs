using Plato.Entities.Models;

namespace Plato.Entities.History.ViewModels
{
    public class HistoryMenuViewModel
    {

        public IEntity Entity { get; set; }

        public IEntityReply Reply { get; set; }

        public string DialogUrl { get; set; }

    }

}
