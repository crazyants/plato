using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Assets.Abstractions
{
    public interface IAssetManager
    {

        Task<IEnumerable<AssetEnvironment>> GetAssets();

    }


}
