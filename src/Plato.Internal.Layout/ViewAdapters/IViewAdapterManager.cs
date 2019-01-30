using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdapters
{
    public interface IViewAdapterManager
    {
        Task<IEnumerable<IViewAdapterResult>> GetViewAdapters(string name);
    }

}
