namespace Plato.Internal.Abstractions.Settings
{

    /// <summary>
    /// Represents core site specific settings.
    /// </summary>
    public class SiteOptions
    {

        public string SiteName { get; set; }

        public string Culture { get; set; }

        public string DateTimeFormat { get; set; }

        public string Theme { get; set; }

    }
}
