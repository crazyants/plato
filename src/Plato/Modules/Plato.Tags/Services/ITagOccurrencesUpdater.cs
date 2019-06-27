using System.Threading.Tasks;
using Plato.Tags.Models;

namespace Plato.Tags.Services
{
    public interface ITagOccurrencesUpdater<in TModel> where TModel : class, ITag
    {
        Task UpdateAsync(TModel tag);
    }

}
