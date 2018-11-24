using System;
using System.Threading.Tasks;

namespace Plato.Badges.Services
{
    public interface IBadgesAwarderManager
    {

        void Award(ref IServiceProvider serviceprovider);
        
    }

}
