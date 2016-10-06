using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.DotNet.ProjectModel.Resolution;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.DotNet.ProjectModel;

namespace Plato.Environment.Modules
{

    //public class CompositeModuleProvider : DefaultAssemblyProvider
    //{

    //    private readonly IAssemblyProvider[] _additionalProviders;
    //    private readonly string[] _referenceAssemblies;
        
    //    public CompositeModuleProvider(
    //        LibraryManager libraryManager,
    //        IAssemblyProvider[] additionalProviders = null,
    //        string[] referenceAssemblies = null) : 
    //        base(libraryManager)
    //    {
    //        _additionalProviders = additionalProviders;
    //        _referenceAssemblies = referenceAssemblies;
    //    }

    //    //        /// <summary>
    //    //        /// Uses the default filter if a custom list of reference
    //    //        /// assemblies has not been provided
    //    //        /// </summary>
    //    //        protected override HashSet<string> ReferenceAssemblies
    //    //            => _referenceAssemblies == null
    //    //                ? base.ReferenceAssemblies
    //    //                : new HashSet<string>(_referenceAssemblies);

    //    //        /// <summary>
    //    //        /// Returns the base Libraries referenced along with any DLLs/Libraries
    //    //        /// returned from the custom IAssemblyProvider passed in
    //    //        /// </summary>
    //    //        /// <returns></returns>
    //    //        protected override IEnumerable<Library> GetCandidateLibraries()
    //    //        {
    //    //            var baseCandidates = base.GetCandidateLibraries();
    //    //            if (_additionalProviders == null) return baseCandidates;
    //    //            return baseCandidates
    //    //                .Concat(
    //    //                _additionalProviders.SelectMany(provider => provider.CandidateAssemblies.Select(
    //    //                    x => new Library(x.FullName, null, Path.GetDirectoryName(x.Location), null, Enumerable.Empty<string>(),
    //    //                        new[] { new AssemblyName(x.FullName) }))));
    //    //        }

    //}

}
