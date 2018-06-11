using System;
using System.Collections.Generic;
using Plato.Internal.Theming.Models;

namespace Plato.Internal.Theming.Locator
{
    public interface IThemeLocator
    {

        IEnumerable<IThemeDescriptor> LocateThemes(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
