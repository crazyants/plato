using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Abstract;
using Plato.Internal.Repositories.Abstract;

namespace Plato.Internal.Stores.Abstract
{

    public class DictionaryStore : IDictionaryStore
    {

        #region "Private Variables"

        private readonly IDictionaryRepository<DictionaryEntry> _dictionaryRepository;
         
        #endregion
        
        #region ""Constructor"

        public DictionaryStore(
            IDictionaryRepository<DictionaryEntry> dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        #endregion

        #region "Implemention"

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var result = await GetByKeyAsync(key);
            if (result != null)
            {
                var serializable = typeof(T) as ISerializable;
                if (serializable != null)
                {
                    return await serializable.DeserializeGenericTypeAsync<T>(result.Value);
                }
                return await result.Value.DeserializeAsync<T>();

            }      
           
            return default(T);
        }
        
        public async Task<T> UpdateAsync<T>(string key, ISerializable value) where T : class
        {

            // Get or create setting
            var existingSetting = await GetByKeyAsync(key)
                                  ?? new DictionaryEntry()
                                  {
                                      Key = key
                                  };

            // Serilize value
            existingSetting.Value = await value.SerializeAsync();

            // update setting & return updated typed settings 
            var updatedSetting = await _dictionaryRepository.InsertUpdateAsync(existingSetting);
            if (updatedSetting != null)
            {
                return await GetAsync<T>(updatedSetting.Key);
            }         
                
            return default(T);

        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _dictionaryRepository.DeleteByKeyAsync(key);
        }

        #endregion

        #region "Private Methods"
        
        private async Task<DictionaryEntry> GetByKeyAsync(string key)
        {
            var allKeys = await GetAllKeys();
            return allKeys?.FirstOrDefault(s => s.Key == key);
        }

        private async Task<IEnumerable<DictionaryEntry>> GetAllKeys()
        {
            var output = new List<DictionaryEntry>();
            var settings = await _dictionaryRepository.SelectEntries();
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    output.Add(setting);
                }
            }
            return output;
        }

        #endregion

    }

}
