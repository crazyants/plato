using System.Collections.Generic;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();
             
        void SaveSettings(ShellSettings settings);

    }

}
