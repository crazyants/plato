using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication.Internal;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
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

        #endregion

        public async Task<TModel> GetAsync<TModel>(IDocument document)
        {
            var entry = await GetEntryByDocumentType(document.GetType());
            return await GetDocumentFromEntryAsync<TModel>(entry);
        }

        public async Task<TModel> UpdateAsync<TModel>(IDocument document)
        {

            // Attempt to get existing document entry by type
            var entry = await GetEntryByDocumentType(document.GetType())
                        ?? new DocumentEntry()
                        {
                            Type = typeof(TModel).ToString()
                        };

            // update entry with serilaized document
            entry.Value = document.Serialize();

            entry.CreatedDate = entry.Id == 0 ? System.DateTime.Now : entry.CreatedDate;
            entry.ModifiedDate = System.DateTime.Now;

            // Add or update entry
            var addedEntry = await _documentRepository.UpdateAsync(entry);
            if (addedEntry != null)
            {

                // Get document from entry
                var entryDocument = (IDocument) await GetDocumentFromEntryAsync<TModel>(addedEntry);

                // Ensure the document has the same unique Id as the entry
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

        public Task<TModel> DeleteAsync<TModel>(IDocument document)
        {
            throw new NotImplementedException();
        }


        async Task<DocumentEntry> GetEntryByDocumentType(Type type)
        {
            return await _documentRepository.GetByType(type.ToString());
        }

        async Task<TModel> GetDocumentFromEntryAsync<TModel>(DocumentEntry entry)
        {
            return await entry.Value.DeserializeAsync<TModel>();
        }


    }
}
