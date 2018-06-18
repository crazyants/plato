using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Features
{

    public interface IFeatureEventHandler
    {

        Task InstallingAsync(object sender, ShellFeatureEventArgs args);

        Task InstalledAsync(object sender, ShellFeatureEventArgs args);

        Task UninstallingAsync(object sender, ShellFeatureEventArgs args);

        Task UninstalledAsync(object sender, ShellFeatureEventArgs args);

    }

}
