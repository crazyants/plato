using System;
using System.Threading.Tasks;
using Plato.Modules.Abstractions;

namespace Plato.Modules
{
    public interface ITypedModuleProvider
    {
        Task<IModuleEntry> GetModuleForDependency(Type dependency);

    }
}
