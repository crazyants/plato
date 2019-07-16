using Plato.Entities.ViewModels;
using Plato.StopForumSpam.Models;

namespace Plato.Questions.StopForumSpam.ViewModels
{
    public class StopForumSpamViewModel
    {

        public EntityOptions Options { get; set; }
        
        public ISpamCheckerResult Checker { get; set; }

    }
}
