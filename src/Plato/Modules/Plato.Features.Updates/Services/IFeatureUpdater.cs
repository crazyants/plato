using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Features.Updates.Services
{

    public interface IFeatureUpdater
    {
        Task<ICommandResultBase> UpdateAsync(string featureId);
    }

}
