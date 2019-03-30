using System;
using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions.Locator
{
    public interface IThemeLocator
    {

        IEnumerable<IThemeDescriptor> LocateThemes(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
