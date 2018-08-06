using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Localization.Abstractions
{
    public interface ITimeZoneProvider<TTimeZone> where TTimeZone : class, ITimeZone
    {
        Task<IEnumerable<TTimeZone>> GetTimeZonesAsync();
    }

}
