using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Repositories.Settings
{
    public interface ISettingsFactory
    {

        IDictionary<string, string> Settings { get; }

        string TryGetValue(string key);

        Task<ISettingsFactory> SelectById(int Id);

        Task<ISettingsFactory> SelectBySiteId(int SiteId);

    }
}
