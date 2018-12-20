using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Models;

namespace Plato.Discuss.History.ViewModels
{
    public class HistoryMenuViewModel
    {

        public IEntity Topic { get; set; }

        public IEntityReply Reply { get; set; }


    }
}
