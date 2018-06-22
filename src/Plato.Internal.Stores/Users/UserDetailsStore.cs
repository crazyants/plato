using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Stores.Users
{

    public interface IUserDetailsStore
    {

        Task<UserDetail> GetAsync(int userId);

        Task<UserDetail> UpdateAsync(int userId, UserDetail value);

        Task<bool> DeleteAsync(int userId);

    }

    public class UserDetailsStore : IUserDetailsStore
    {

        public const string Key = "Plato.Users.Details";

        private readonly IUserDataStore<UserData> _userDataStore;

        public UserDetailsStore(
            IUserDataStore<UserData> userDataStore)
        {
            _userDataStore = userDataStore;
        }

        public async Task<UserDetail> GetAsync(int userId)
        {
            return await _userDataStore.GetAsync<UserDetail>(userId, Key);
        }

        public async Task<UserDetail> UpdateAsync(int userId, UserDetail value)
        {
            return await _userDataStore.UpdateAsync<UserDetail>(userId, Key, value);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            return await _userDataStore.DeleteAsync(userId, Key);
        }
    }
}
