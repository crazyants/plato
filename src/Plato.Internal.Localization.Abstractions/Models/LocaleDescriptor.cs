using System.Collections.Generic;
using System.IO;

namespace Plato.Internal.Localization.Abstractions.Models
{
    public class LocaleDescriptor
    {

        public string Name { get; set; }

        public string Path { get; set; }

        public DirectoryInfo DirectoryInfo { get; set; }

        public IEnumerable<LocaleResource> Resources { get; set; }
        
    }

}
