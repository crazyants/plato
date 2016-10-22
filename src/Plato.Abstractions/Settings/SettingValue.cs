using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Settings
{
    public class SettingValue : ISettingValue 
    {

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
