using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Features.Updates.Services
{

    public interface IFeatureUpdater
    {
   
        Task<ICommandResultBase> UpdateAsync(string featureId);
    }

    public class FeatureUpdater : IFeatureUpdater
    {
        public Task<ICommandResultBase> UpdateAsync(string featureId)
        {

            var result = new CommandResultBase();
            
            return Task.FromResult((ICommandResultBase) result.Success());

        }

    }

}
