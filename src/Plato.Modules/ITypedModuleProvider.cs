using System;
using Plato.Modules.Abstractions;

namespace Plato.Modules
{
    public interface ITypedModuleProvider
    {
        IModuleEntry GetModuleForDependency(Type dependency);
    }
}
