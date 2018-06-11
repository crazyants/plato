using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Plato.Internal.Abstractions.Settings
{
    public sealed class SiteSettings : ISiteSettings
    {

        public string SiteName { get; set; }

        public string SiteSalt { get; set; }

        public string Calendar { get; set; }

        public string Culture { get; set; }

        public int MaxPagedCount { get; set; }

        public int MaxPageSize { get; set; }

        public int PageSize { get; set; }

        public string PageTitleSeparator { get; set; }
   
        public string SuperUser { get; set; }

        public string TimeZone { get; set; }
        
        public string BaseUrl { get; set; }

        public bool UseCdn { get; set; }

        public RouteValueDictionary HomeRoute { get; set; }

        public string ThemeName { get; set; } = "Default";

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
