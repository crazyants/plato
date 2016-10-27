using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Settings;

namespace Plato.Abstractions.Stores
{ 
    public interface ISiteSettingsStore : ISettingsStore<ISiteSettings>
    {
    }
}
