using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Settings;

namespace Plato.Internal.Repositories.Settings
{
    public interface ISettingRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<Setting>> SelectSettings();
    }

}
