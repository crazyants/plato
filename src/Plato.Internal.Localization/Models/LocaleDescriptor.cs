using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Localization.Models
{
    public class LocaleDescriptor
    {

        public string Id { get; set; }

        public string FullPath { get; set; }

        public IEnumerable<LocaleResource> Resources { get; set; }
        
    }

    public class LocaleResource
    {

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string Contents { get; set; }

    }
}
