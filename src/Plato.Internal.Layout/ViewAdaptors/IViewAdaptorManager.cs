using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdaptors
{
    public interface IViewAdaptorManager
    {
        Task<IEnumerable<IViewAdaptorResult>> GetViewAdaptors(string name);
    }

}
