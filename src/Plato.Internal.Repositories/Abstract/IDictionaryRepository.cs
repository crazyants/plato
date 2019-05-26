using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Abstract;

namespace Plato.Internal.Repositories.Abstract
{
    public interface IDictionaryRepository<T> : IRepository2<T> where T : class
    {
        Task<IEnumerable<DictionaryEntry>> SelectEntries();

        Task<DictionaryEntry> SelectEntryByKey(string key);
        
        Task<bool> DeleteByKeyAsync(string key);

    }

}
