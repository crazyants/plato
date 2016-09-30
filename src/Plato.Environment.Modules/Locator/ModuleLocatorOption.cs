using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public interface IModuleLocatorExpander
    {
        string ModuleType { get; }
        string ManifestName { get; }
        bool ManifestOptional { get; }
        string[] SearchPaths { get; }
    }

    public class ModuleLocatorExpander :
        IModuleLocatorExpander
    {
        public ModuleLocatorExpander(
            string extensionType,
            string[] searchPaths,
            string manifestName,
            bool manifestOptional = false)
        {
            ModuleType = extensionType;
            SearchPaths = searchPaths;
            ManifestName = manifestName;
            ManifestOptional = manifestOptional;
        }

        public string ModuleType { get; private set; }
        public string[] SearchPaths { get; private set; }
        public string ManifestName { get; private set; }
        public bool ManifestOptional { get; private set; }
    }
}
