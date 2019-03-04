using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Services
{

    
    public interface IReportManager<TModel> where TModel : class
    {

        Task<ICommandResult<TModel>> ReportAsync(TModel model, IUser reportedBy);
        
    }


    public class ReportTopicManager : IReportManager<Topic> 
    {
        public Task<ICommandResult<Topic>> ReportAsync(Topic model, IUser reportedBy)
        {
      
            var result = new CommandResult<Topic>();

            return Task.FromResult(result.Success(model));
        }
    }

    public class ReportReplyManager : IReportManager<Reply>
    {

        public Task<ICommandResult<Reply>> ReportAsync(Reply model, IUser reportedBy)
        {
            var result = new CommandResult<Reply>();

            return Task.FromResult(result.Success(model));
        }

    }


}
