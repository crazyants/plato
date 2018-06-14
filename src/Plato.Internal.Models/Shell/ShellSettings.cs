using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Models.Shell
{
    public class ShellSettings : IShellSettings
    {
        
        private readonly IDictionary<string, string> _values;

        public ShellSettings() : this(new Dictionary<string, string>()) { }

        public ShellSettings(IDictionary<string, string> configuration)
        {
            _values = new Dictionary<string, string>(configuration);

            if (!configuration.ContainsKey("State") || !Enum.TryParse(configuration["State"], true, out _tenantState))
            {
                _tenantState = TenantState.Invalid;
            }
        }

        public ShellSettings(ShellSettings settings)
        {
            _values = new Dictionary<string, string>(settings._values, StringComparer.OrdinalIgnoreCase);
            Name = settings.Name;
            RequestedUrlHost = settings.RequestedUrlHost;
            RequestedUrlPrefix = settings.RequestedUrlPrefix;      
            DatabaseProvider = settings.DatabaseProvider;
            ConnectionString = settings.ConnectionString;
            TablePrefix = settings.TablePrefix;           
            Theme = settings.Theme;
            State = settings.State;

        }

        public IDictionary<string, string> Configuration => _values;
        
        public string this[string key]
        {
            get => _values.TryGetValue(key, out var retVal) ? retVal : null;
            set => _values[key] = value;
        }

        public IEnumerable<string> Keys => _values.Keys;

        public string Name
        {
            get => this["Name"] ?? "";
            set => this["Name"] = value;
        }

        public string Location
        {
            get => this["Location"] ?? "";
            set => this["Location"] = value;
        }

        public string RequestedUrlHost
        {
            get => this["RequestedUrlHost"];
            set => this["RequestedUrlHost"] = value;
        }

        public string RequestedUrlPrefix
        {
            get => this["RequestedUrlPrefix"];
            set => _values["RequestedUrlPrefix"] = value;
        }

        public string ConnectionString
        {
            get => this["ConnectionString"];
            set => _values["ConnectionString"] = value;
        }

        public string TablePrefix
        {
            get => this["TablePrefix"];
            set => _values["TablePrefix"] = value;
        }

        public string DatabaseProvider
        {
            get => this["DatabaseProvider"];
            set => _values["DatabaseProvider"] = value;
        }

        public string Theme
        {
            get => this["Theme"];
            set => _values["Theme"] = value;
        }

        public string AuthCookieName
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var c in this.Name)
                {
                    if ((char.IsLetter(c)) || ((char.IsNumber(c))))
                        sb.Append(c);
                }
                return !string.IsNullOrEmpty(sb.ToString()) 
                    ? sb.ToString() 
                    : this.Name;
            }
        }

        TenantState _tenantState;

        public TenantState State
        {
            get => _tenantState;
            set
            {
                _tenantState = value;
                this["State"] = value.ToString();
            }
        }
    }

}
