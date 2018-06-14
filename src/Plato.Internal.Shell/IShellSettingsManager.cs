using System.Collections.Generic;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Internal.Shell
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();
             
        void SaveSettings(ShellSettings settings);

    }

}
