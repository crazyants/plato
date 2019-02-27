using System.Collections.Generic;
using Plato.Internal.Theming.Models;

namespace Plato.Internal.Theming
{
    public interface IThemeManager
    {

        IEnumerable<IThemeDescriptor> AvailableThemes { get; }

    }

}
