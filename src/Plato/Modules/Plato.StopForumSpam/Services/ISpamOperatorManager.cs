using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperatorManager<TModel> where TModel : class
    {

        Task<IEnumerable<ISpamOperatorResult<TModel>>> ValidateModelAsync(ISpamOperation operation, TModel model);

        Task<IEnumerable<ISpamOperatorResult<TModel>>> UpdateModelAsync(ISpamOperation operation, TModel model);
    }

}
