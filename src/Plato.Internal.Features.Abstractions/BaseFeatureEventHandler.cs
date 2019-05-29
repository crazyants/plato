using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.Features.Abstractions
{
    public abstract class BaseFeatureEventHandler : IFeatureEventHandler
    {

        public string ModuleId => Path.GetFileNameWithoutExtension(this.GetType().Assembly.ManifestModule.Name);

        public virtual Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task UninstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task UpdatingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task UpdatedAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

    }

}
