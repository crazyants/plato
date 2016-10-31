using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Repositories.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Plato.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore
    {

        private readonly string _key = CacheKeys.Users.ToString();

        private readonly IUserRepository<User> _userRepository;
        private readonly IMemoryCache _memoryCache;

        public PlatoUserStore(
            IUserRepository<User> userRepository,
            IMemoryCache memoryCache
            )
        {
            _userRepository = userRepository;
            _memoryCache = memoryCache;
        }
        
        public Task<User> CreateAsync(User model)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteAsync(User model)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            User user;
            if (!_memoryCache.TryGetValue(_key, out user))
            {
                user = await _userRepository.SelectByIdAsync(id);
                if (user != null)
                    _memoryCache.Set(_key, user);
            }

            return user;

        }

        public IQuery QueryAsync()
        {
            return new UserQuery(this);
        }

        public Task<User> UpdateAsync(User model)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> SelectAsync<T>(params object[] args) where T : class
        {

            IEnumerable<T> users;
            
            if (!_memoryCache.TryGetValue(_key, out users))
            {
                users = await _userRepository.SelectAsync<T>(args));
                if (users != null)
                    _memoryCache.Set(_key, users.ToList());
            }

            return users;
            

        }


    }
}
