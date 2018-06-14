using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Modules;

namespace Plato.Internal.Modules.Abstractions
{
    public interface ITypedModuleProvider
    {
        Task<IModuleEntry> GetModuleForDependency(Type dependency);

    }
}
