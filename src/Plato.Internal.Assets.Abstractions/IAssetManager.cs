using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Assets.Abstractions
{
    public interface IAssetManager
    {

        Task<IEnumerable<AssetEnvironment>> GetAssets();

        void SetAssets(IEnumerable<AssetEnvironment> environments);

    }


}
