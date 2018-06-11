using System;
using System.Threading.Tasks;

namespace Plato.Internal.Modules.Abstractions
{
    public interface ITypedModuleProvider
    {
        Task<IModuleEntry> GetModuleForDependency(Type dependency);

    }
}
