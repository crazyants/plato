using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeLoader
    {
        string RootPath { get;  }

        IEnumerable<IThemeDescriptor> AvailableThemes { get; }
    
    }

}
