using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Settings;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Abstractions.Settings;

namespace Plato.Internal.Repositories.Abstract
{
    public class DictionaryFactory : IDictionaryFactory
    {

        #region "Private Variables"

        private readonly IDictionaryRepository<DictionaryEntry> _dictionaryRepository;
         
        #endregion
        
        #region ""Constructor"

        public DictionaryFactory(
            IDictionaryRepository<DictionaryEntry> dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        #endregion

        #region "Implemention"

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await GetByKeyAsync(key);
            if (result != null)            
                return await result.Value.DeserializeAsync<T>();
            return default(T);
        }
        
        public async Task<T> UpdateAsync<T>(string key, ISerializable value)
        {

            // Get or create setting
            var existingSetting = await GetByKeyAsync(key)
                                  ?? new DictionaryEntry()
                                  {
                                      Key = key
                                  };

            // Serilize value
            existingSetting.Value = value.Serialize();

            // update setting & return updated typed settings 
            var updatedSetting = await _dictionaryRepository.InsertUpdateAsync(existingSetting);
            if (updatedSetting != null)
            {
                return await GetAsync<T>(updatedSetting.Key);
            }         
                
            return default(T);

        }

        public async Task<bool> DeleteByKeyAsync(string key)
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
                   output.Add(setting);  
            }
            return output;
        }

        #endregion
        
    }
}
