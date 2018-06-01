using System;
using System.Collections.Generic;
using Plato.Theming.Models;

namespace Plato.Theming
{
    public interface IThemeManager
    {

        IEnumerable<IThemeDescriptor> AvailableThemes { get; }

    }
}
