namespace Plato.Internal.Abstractions.Settings
{

    /// <summary>
    /// Represents core application settings.
    /// </summary>
    public class PlatoOptions
    {
        public string Version { get; set; }

        public string ReleaseType { get; set; }

        public bool DemoMode { get; set; }

    }

}
