using System;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Notifications.Repositories
{

    public interface IUserNotificationsRepository<T> : IRepository<T> where T : class
    {
        Task<bool> UpdateReadDateAsync(int userId, DateTimeOffset? readDate);

    }

}
