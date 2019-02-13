using System.Threading.Tasks;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface ISpamOperatorProvider<TModel> where TModel : class
    {
        Task<ISpamOperatorResult<TModel>> OperateAsync(ISpamOperatorContext<TModel> context);
    }
    
}
