using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Models.Settings;

namespace Plato.Repositories.Settings
{
    public interface ISettingRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<Setting>> SelectSettings();
                

    }

}
