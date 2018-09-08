using Microsoft.Extensions.FileProviders;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class LocaleResource
    {

        public string Name { get; set; }

        public string Path { get; set; }

        public string Location { get; set; }

        public IFileInfo FileInfo { get; set; }

        public string Contents { get; set; }

    }

}
