using Plato.Entities.ViewModels;
using Plato.StopForumSpam.Models;

namespace Plato.Issues.StopForumSpam.ViewModels
{
    public class StopForumSpamViewModel
    {

        public EntityOptions Options { get; set; }
        
        public ISpamCheckerResult Checker { get; set; }

    }
}
