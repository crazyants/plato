using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Notifications.Repositories
{

    public interface IUserNotificationsRepository<T> : IRepository<T> where T : class
    {
        

    }

}
