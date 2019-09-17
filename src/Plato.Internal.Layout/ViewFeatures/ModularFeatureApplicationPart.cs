using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.ViewFeatures
{

    public class ModularFeatureApplicationPart :
        ApplicationPart,
        IApplicationPartTypeProvider,
        ICompilationReferencesProvider
    {

        private static IEnumerable<string> _referencePaths;
        private static object _synLock = new object();
 
        private readonly ITypedModuleProvider _typedModuleProvider;
 
        public ModularFeatureApplicationPart(IServiceProvider services)
        {                  
            _typedModuleProvider = services.GetRequiredService<ITypedModuleProvider>();
        }

        public override string Name
        {
            get
            {
                return nameof(ModularFeatureApplicationPart);
            }
        }

        /// <inheritdoc />
        public IEnumerable<TypeInfo> Types
        {
            get
            {
                return _typedModuleProvider.GetTypesAsync()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetReferencePaths()
        {
            if (_referencePaths != null)
            {
                return _referencePaths;
            }

            lock (_synLock)
            {
                if (_referencePaths != null)
                {
                    return _referencePaths;
                }

                var types = this.Types;

                _referencePaths = DependencyContext.Default.CompileLibraries
                    .SelectMany(library => library.ResolveReferencePaths());
            }

            return _referencePaths;

        }

    }

}
