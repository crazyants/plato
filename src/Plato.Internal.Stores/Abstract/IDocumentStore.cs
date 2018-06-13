using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models;
using Plato.Internal.Models.Abstract;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Abstract
{
    public interface IDocumentStore
    {

        Task<TModel> GetAsync<TModel>(IDocument document);

        Task<TModel> UpdateAsync<TModel>(IDocument document);

        Task<TModel> DeleteAsync<TModel>(IDocument document);


    }
}
