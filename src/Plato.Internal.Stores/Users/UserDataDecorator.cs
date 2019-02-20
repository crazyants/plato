using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    
    public class UserDataDecorator : IUserDataDecorator
    {

        private readonly IUserDataStore<UserData> _userDataStore;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public UserDataDecorator(
            IUserDataStore<UserData> userDataStore,
            ITypedModuleProvider typedModuleProvider)
        {
            _userDataStore = userDataStore;
            _typedModuleProvider = typedModuleProvider;
        }

        public  async Task<IEnumerable<User>> DecorateAsync(IEnumerable<User> users)
        {
            if (users == null)
            {
                return null;
            }

            // Get all user data matching supplied users
            var results = await _userDataStore.QueryAsync()
                .Select<UserDataQueryParams>(q =>
                {
                    q.UserId.IsIn(users.Select(u => u.Id).ToArray());
                })
                .ToList();

            if (results == null)
            {
                return users;
            }

            // Merge data into users
            return await MergeData(users.ToList(), results.Data);
        }
        
        public async Task<User> DecorateAsync(User user)
        {

            if (user == null)
            {
                return null;
            }

            if (user.Data == null)
            {
                return user;
            }

            foreach (var data in user.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    user.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return user;

        }
  
        async Task<IList<User>> MergeData(IList<User> users, IList<UserData> data)
        {

            if (users == null || data == null)
            {
                return users;
            }

            for (var i = 0; i < users.Count; i++)
            {
                users[i].Data = data.Where(d => d.UserId == users[i].Id).ToList();
                users[i] = await DecorateAsync(users[i]);
            }

            return users;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

    }

}
