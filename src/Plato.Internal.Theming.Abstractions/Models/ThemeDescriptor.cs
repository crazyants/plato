using System;
using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions.Models
{
    public class ThemeDescriptor : IThemeDescriptor
    {
        
        private readonly IDictionary<string, string> _values;

        public ThemeDescriptor() : this(new Dictionary<string, string>())
        {
        }

        public ThemeDescriptor(IDictionary<string, string> configuration)
        {
            _values = new Dictionary<string, string>(configuration);
        }

        public ThemeDescriptor(ThemeDescriptor descriptor)
        {
            _values = new Dictionary<string, string>(descriptor._values, StringComparer.OrdinalIgnoreCase);
            Id = descriptor.Id;
            Name = descriptor.Name;
            Description = descriptor.Description;
            Author = descriptor.Author;
            AuthorUrl = descriptor.AuthorUrl;
            WebSite = descriptor.WebSite;
            Version = descriptor.Version;
            PlatoVersion = descriptor.PlatoVersion;
            Category = descriptor.Category;
            FullPath = descriptor.FullPath;
        }

        public IDictionary<string, string> Configuration => _values;

        public string this[string key]
        {
            get => _values.TryGetValue(key, out var retVal) ? retVal : null;
            set => _values[key] = value;
        }

        public IEnumerable<string> Keys => _values.Keys;

        public string Id { get; set; }

        public string FullPath { get; set; }

        public string Name
        {
            get => this["Name"] ?? "";
            set => this["Name"] = value;
        }

        public string Description
        {
            get => this["Description"] ?? "";
            set => this["Description"] = value;
        }

        public string Author
        {
            get => this["Author"] ?? "";
            set => this["Author"] = value;
        }

        public string AuthorUrl
        {
            get => this["AuthorUrl"] ?? "";
            set => this["AuthorUrl"] = value;
        }

        public string WebSite
        {
            get => this["WebSite"] ?? "";
            set => this["WebSite"] = value;
        }

        public string Version
        {
            get => this["Version"];
            set => _values["Version"] = value;
        }

        public string PlatoVersion
        {
            get => this["PlatoVersion"];
            set => _values["PlatoVersion"] = value;
        }

        public string Category
        {
            get => this["Category"];
            set => _values["Category"] = value;
        }

        public string Tags
        {
            get => this["Tags"];
            set => _values["Tags"] = value;
        }

    }

}
