using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;

namespace Plato.Internal.Stores.Users
{
    public class UserDataStore : IUserDataStore<UserData>
    {

        #region "Private Variables"

        private readonly IUserDataRepository<UserData> _userDataRepository;

        #endregion

        #region ""Constructor"

        public UserDataStore(
            IUserDataRepository<UserData> userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        #endregion

        #region "IMplementation"

        public async Task<T> GetAsync<T>(int userId, string key)
        {
            var result = await GetByUserIdAndKeyAsync(userId, key);
            if (result != null)
            {
                return await result.Value.DeserializeAsync<T>();
            }

            return default(T);
        }

        public async Task<T> UpdateAsync<T>(int userId, string key, ISerializable value)
        {

            // Get or create setting
            var data = await GetByUserIdAndKeyAsync(userId, key)
                       ?? new UserData()
                       {
                           UserId = userId,
                           Key = key
                       };

            // Serilize value
            data.Value = value.Serialize();

            var updatedData = await _userDataRepository.InsertUpdateAsync(data);
            if (updatedData != null)
            {
                return await GetAsync<T>(userId, updatedData.Key);
            }

            return default(T);
        }

        public Task<bool> DeleteAsync(int userId, string key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<UserData> GetByUserIdAndKeyAsync(int userId, string key)
        {
            var allKeys = await GetAllKeys(userId);
            return allKeys?.FirstOrDefault(s => s.Key == key);
        }

        private async Task<IEnumerable<UserData>> GetAllKeys(int userId)
        {
            var output = new List<UserData>();
            var entries = await _userDataRepository.SelectDataByUserId(userId);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    output.Add(entry);
                }
            }

            return output;
        }

        #endregion

    }

}
