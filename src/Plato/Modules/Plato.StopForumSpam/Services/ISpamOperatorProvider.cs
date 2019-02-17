using System.Threading.Tasks;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface ISpamOperatorProvider<TModel> where TModel : class
    {

        Task<ISpamOperatorResult<TModel>> ValidateModelAsync(ISpamOperatorContext<TModel> context);

        Task<ISpamOperatorResult<TModel>> UpdateModelAsync(ISpamOperatorContext<TModel> context);

    }
    
}
