using System;

namespace Plato.Internal.Theming.Abstractions.Models
{
   public class ThemeDescriptor : IThemeDescriptor
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string WebSite { get; set; }

        public string Version { get; set; } = "1.0.0";

        public string PlatoVersion { get; set; }

        public string AuthorUrl { get; set; }

        public string Category { get; set; }

        public string FullPath { get; set; }

    }

}
