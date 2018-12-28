using Microsoft.AspNetCore.Mvc.RazorPages;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class TopicReplyListItemViewModel
    {

        public Reply Reply { get; set;  }

        public int Index { get; set; }

    }
}
