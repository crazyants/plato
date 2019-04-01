using Plato.Internal.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeDescriptorUpdater
    {

        ICommandResult<IThemeDescriptor> UpdateThemeDescriptor(string pathToThemeFolder, IThemeDescriptor descriptor);

    }
}
