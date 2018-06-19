using System.Collections.Generic;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();
             
        void SaveSettings(IShellSettings settings);

    }

}
