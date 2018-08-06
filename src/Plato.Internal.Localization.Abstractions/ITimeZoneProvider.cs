using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Plato.Internal.Localization.Abstractions
{
    public interface ITimeZoneProvider
    {
        Task<ReadOnlyCollection<TimeZoneInfo>> GetTimeZonesAsync();

        Task<TimeZoneInfo> GetTimeZoneByIdAsync(string id);

    }

}
