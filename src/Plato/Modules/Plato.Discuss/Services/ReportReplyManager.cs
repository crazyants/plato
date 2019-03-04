using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Services
{

    public class ReportReplyManager : IReportEntityManager<Reply>
    {

        public Task<ICommandResult<Reply>> ReportAsync(ReportSubmission<Reply> submission)
        {
            var result = new CommandResult<Reply>();

            return Task.FromResult(result.Success(submission.What));
        }

    }

}
