using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;
using Plato.Internal.Models.Abstract;
using Plato.Internal.Repositories.Abstract;

namespace Plato.Internal.Stores.Abstract
{
    public class DocumentStore : IDocumentStore
    {
        
        #region "Private Variables"

        private readonly IDocumentRepository _documentRepository;

        #endregion

        #region ""Constructor"

        public DocumentStore(
            IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        #endregion

        #region "Implemention"
        
        public async Task<TModel> GetAsync<TModel>()
        {
            var entry = await GetEntryByDocumentType(typeof(TModel));
            return await GetDocumentFromEntryAsync<TModel>(entry);
        }

        public async Task<TModel> SaveAsync<TModel>(IDocument document)
        {

            // Attempt to get existing document entry or create a new one
            var entry = await GetEntryByDocumentType(document.GetType())
                        ?? new DocumentEntry()
                        {
                            Type = typeof(TModel).ToString()
                        };

            // Update entry with latest document
            entry.Value = await document.SerializeAsync();

            // Set meta data
            entry.CreatedDate = entry.Id == 0 ? System.DateTime.Now : entry.CreatedDate;
            entry.ModifiedDate = System.DateTime.Now;

            // Add or update entry
            var addedEntry = await _documentRepository.UpdateAsync(entry);
            if (addedEntry != null)
            {

                // Ensure the document has the same unique Id as the entry
                // This ensures the document has a unique Id to work with if needed
                // This is only executed the first time a type is added
                var entryDocument = (IDocument) await GetDocumentFromEntryAsync<TModel>(addedEntry);
                if (entryDocument.Id == 0)
                {
                    entryDocument.Id = addedEntry.Id;
                    addedEntry.Value = entryDocument.Serialize();
                    var updatedEntry = await _documentRepository.UpdateAsync(addedEntry);
                    {
                        return await GetDocumentFromEntryAsync<TModel>(updatedEntry);
                    }
                }
                
                return await GetDocumentFromEntryAsync<TModel>(addedEntry);

            }

            return default(TModel);

        }

        public async Task<bool> DeleteAsync<TModel>()
        {
            var success = false;
            // Ensure we have an entry for the type
            var entry = await GetEntryByDocumentType(typeof(TModel));
            if (entry != null)
            {
                success = await _documentRepository.DeleteAsync(entry.Id);

            }

            return success;
        }

        #endregion

        #region "Private Methods"
        
        async Task<DocumentEntry> GetEntryByDocumentType(Type type)
        {
            return await _documentRepository.GetByType(type.ToString());
        }

        async Task<TModel> GetDocumentFromEntryAsync<TModel>(DocumentEntry entry)
        {

            if (entry == null)
            {
                return default(TModel);
            }

            if (String.IsNullOrEmpty(entry.Value))
            {
                return default(TModel);
            }

            var serializable = typeof(TModel) as ISerializable;
            if (serializable != null)
            {
                return await serializable.DeserializeGenericTypeAsync<TModel>(entry.Value);
            }

            return await entry.Value.DeserializeAsync<TModel>();
        }
        
        #endregion

    }
}
