using System.Collections.Generic;
using Plato.Internal.Shell.Models;

namespace Plato.Internal.Shell
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();
             
        void SaveSettings(ShellSettings settings);

    }

}
