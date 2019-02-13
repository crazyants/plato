using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperatorManager<TModel> where TModel : class
    {

        Task<IEnumerable<ISpamOperatorResult<TModel>>> OperateAsync(ISpamOperation operation, TModel model);

    }

}
