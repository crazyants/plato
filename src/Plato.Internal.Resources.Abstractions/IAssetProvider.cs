using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Assets.Abstractions
{

    public interface IAssetProvider
    {
        Task<IEnumerable<AssetEnvironment>> GetResourceGroups();

    }

}
