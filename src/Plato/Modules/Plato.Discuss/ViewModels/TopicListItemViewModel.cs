using System;
using System.Collections.Generic;
using System.Text;
using Plato.Discuss.Models;

namespace Plato.Discuss.ViewModels
{
    public class TopicListItemViewModel
    {

        public Topic Topic { get; set; }

        public bool EnableEditOptions { get; set; }
    }
}
