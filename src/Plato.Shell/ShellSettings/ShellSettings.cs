using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public class ShellSettings
    {
        
        private readonly IDictionary<string, string> _values;

        public ShellSettings()
        {
            _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);         
        }

        public ShellSettings(ShellSettings settings)
        {
            _values = new Dictionary<string, string>(settings._values, StringComparer.OrdinalIgnoreCase);

            Name = settings.Name;
            HostName = settings.HostName;
            SubDomain = settings.SubDomain;
            State = settings.State;
            DatabaseProvider = settings.DatabaseProvider;
            ConnectionString = settings.ConnectionString;
            TablePrefix = settings.TablePrefix;
            State = settings.State;
        }

        public string this[string key]
        {
            get
            {
                string retVal;
                return _values.TryGetValue(key, out retVal) ? retVal : null;
            }
            set { _values[key] = value; }
        }

        public IEnumerable<string> Keys { get { return _values.Keys; } }

        public string Name
        {
            get { return this["Name"] ?? ""; }
            set { this["Name"] = value; }
        }

        public string Location
        {
            get { return this["Location"] ?? ""; }
            set { this["Location"] = value; }
        }

        public string HostName
        {
            get { return this["RequestUrlHost"]; }
            set { this["RequestUrlHost"] = value; }
        }

        public string SubDomain
        {
            get { return this["RequestUrlPrefix"]; }
            set { _values["RequestUrlPrefix"] = value; }
        }

        public string ConnectionString
        {
            get { return this["ConnectionString"]; }
            set { _values["ConnectionString"] = value; }
        }

        public string TablePrefix
        {
            get { return this["TablePrefix"]; }
            set { _values["TablePrefix"] = value; }
        }

        public string DatabaseProvider
        {
            get { return this["DatabaseProvider"]; }
            set { _values["DatabaseProvider"] = value; }
        }

        TenantState _tenantState;

        public TenantState State
        {
            get { return _tenantState; }
            set
            {
                _tenantState = value;
                this["State"] = value.ToString();
            }
        }
    }

}
