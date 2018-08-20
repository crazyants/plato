using System.Threading.Tasks;
using Plato.Internal.Models;
using Plato.Internal.Stores.Abstract;

namespace Plato.Discuss.Moderation.Stores
{

    public interface IModeratorStore<TDocument> where TDocument : class, IDocument
    {

        Task<TDocument> GetAsync();

        Task<TDocument> SaveAsync(IDocument document);

        Task<bool> DeleteAsync();

    }

    public class ModeratorStore<TDocument> : IModeratorStore<TDocument> where TDocument : class, IDocument
    {

        private readonly IDocumentStore _documentStore;

        public ModeratorStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<TDocument> GetAsync()
        {
            return await _documentStore.GetAsync<TDocument>();
        }

        public async Task<TDocument> SaveAsync(IDocument document)
        {
            return await _documentStore.SaveAsync<TDocument>(document);
        }

        public async Task<bool> DeleteAsync() 
        {
            return await _documentStore.DeleteAsync<TDocument>();
        }
        
    }
}
