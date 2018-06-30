using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Stores
{
    
    //public interface IEntityDetailsStore<T> where T : class
    //{

    //    Task<T> GetAsync(int userId);

    //    Task<T> UpdateAsync(int userId, UserDetail value);

    //    Task<bool> DeleteAsync(int userId);

    //}

    //public class EntityDetailsStore : IEntityDetailsStore<EntityDetails>
    //{

    //    public const string Key = "Plato.Users.Details";

    //    private readonly IEntityDataStore<EntityData> _userDataStore;

    //    public EntityDetailsStore(
    //        IEntityDataStore<EntityData> userDataStore)
    //    {
    //        _userDataStore = userDataStore;
    //    }

    //    public async Task<EntityDetails> GetAsync(int userId)
    //    {
    //        return await _userDataStore.GetAsync<EntityDetails>(userId, Key);
    //    }

    //    public async Task<EntityDetails> UpdateAsync(int userId, UserDetail value)
    //    {
    //        return await _userDataStore.UpdateAsync<EntityDetails>(userId, Key, value);
    //    }

    //    public async Task<bool> DeleteAsync(int userId)
    //    {
    //        return await _userDataStore.DeleteAsync(userId, Key);
    //    }
    //}




}
