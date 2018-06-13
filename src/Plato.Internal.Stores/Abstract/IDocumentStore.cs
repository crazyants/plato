using System.Threading.Tasks;
using Plato.Internal.Models;

namespace Plato.Internal.Stores.Abstract
{
    public interface IDocumentStore
    {

        Task<TModel> GetAsync<TModel>();

        Task<TModel> SaveAsync<TModel>(IDocument document);

        Task<bool> DeleteAsync<TModel>();

    }
}
