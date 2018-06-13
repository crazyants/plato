using System;
using System.Collections.Generic;
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
            var entry = await _documentRepository.GetAsync(document.Id);
            if (entry != null)
                return await entry.Value.DeserializeAsync<TModel>();
            return default(TModel);
        }

        public async Task<TModel> UpdateAsync<TModel>(IDocument document)
        {

            var entry = await _documentRepository.GetAsync(document.Id);

            //var doc = await GetAsync<TModel>(document);

            entry.Value = document.Serialize();

            var updatedEntry = await _documentRepository.UpdateAsync(entry);
            if (updatedEntry != null)
            {
                return await GetAsync<TModel>(document);
            }

            return default(TModel);


        }

        public Task<TModel> DeleteAsync<TModel>(IDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
