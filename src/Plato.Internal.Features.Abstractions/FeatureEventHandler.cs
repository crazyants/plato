using System.IO;
using System.Threading.Tasks;

namespace Plato.Internal.Features.Abstractions
{
    public abstract class BaseFeatureEventHandler : IFeatureEventHandler
    {

        public string ModuleId => Path.GetFileNameWithoutExtension(this.GetType().Assembly.ManifestModule.Name);

        public abstract Task InstallingAsync(IFeatureEventContext context);
        
        public abstract Task InstalledAsync(IFeatureEventContext context);

        public abstract Task UninstallingAsync(IFeatureEventContext context);

        public abstract Task UninstalledAsync(IFeatureEventContext context);

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
