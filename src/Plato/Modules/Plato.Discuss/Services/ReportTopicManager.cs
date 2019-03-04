using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Entities.Models;

namespace Plato.Discuss.Services
{
    
    public class ReportTopicManager : IReportEntityManager<Topic> 
    {

        private readonly INotificationManager<Topic> _notificationManager;

        public ReportTopicManager()
        {

        }

        public Task<ICommandResult<Topic>> ReportAsync(ReportSubmission<Topic> submission)
        {
      
            var result = new CommandResult<Topic>();

            return Task.FromResult(result.Success(submission.What));

        }
    }
    
}
