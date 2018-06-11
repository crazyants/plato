using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserSettingsRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<UserSetting>> SelectSettingsByUserId(int userId);

    }
}
