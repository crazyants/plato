using Plato.Internal.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeUpdater
    {

        ICommandResult<IThemeDescriptor> UpdateTheme(string pathToThemeFolder, IThemeDescriptor descriptor);

    }
}
