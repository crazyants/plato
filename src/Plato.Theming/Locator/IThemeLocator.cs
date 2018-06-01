using System;
using System.Collections.Generic;
using Plato.Theming.Models;

namespace Plato.Theming.Locator
{
    public interface IThemeLocator
    {

        IEnumerable<IThemeDescriptor> LocateThemes(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
