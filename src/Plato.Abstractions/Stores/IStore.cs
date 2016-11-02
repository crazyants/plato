using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Abstractions.Query;
using Plato.Abstractions.Collections;

namespace Plato.Abstractions.Stores
{

    public interface IStore<TModel> where TModel : class
    {
 
        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);
        
        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);

        IQuery QueryAsync();

        Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class;


    }
}