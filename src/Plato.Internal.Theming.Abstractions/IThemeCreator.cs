using Plato.Internal.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeCreator
    {
        ICommandResult<IThemeDescriptor> CreateTheme(string baseThemeId, string newThemeName);

    }

}
