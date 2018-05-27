using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Models.Users;

namespace Plato.Repositories.Users
{
    public interface IUserSettingsRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<UserSetting>> SelectSettingsByUserId(int userId);

    }
}
