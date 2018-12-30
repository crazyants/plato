using Microsoft.AspNetCore.Mvc.RazorPages;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class TopicReplyListItemViewModel
    {
        
        public Reply Reply { get; set;  }

        //public int Offset { get; set; }

        public int SelectedOffset { get; set; }

    }
}
