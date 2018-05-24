using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Settings;
using Plato.Abstractions.Stores;

namespace Plato.Settings.Services
{ 
    public interface ISettingsService : ISettingsStore<ISiteSettings>
    {
    }
}
