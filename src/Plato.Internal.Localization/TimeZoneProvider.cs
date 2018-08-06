using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization
{

    public class TimeZoneProvider : ITimeZoneProvider
    {
        private static ReadOnlyCollection<TimeZoneInfo> _timeZones;
   
        public async Task<ReadOnlyCollection<TimeZoneInfo>> GetTimeZonesAsync()
        {
            await PopulateTimeZones();
            return _timeZones;
        }

        public async Task<TimeZoneInfo> GetTimeZoneByIdAsync(string id)
        {

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            await PopulateTimeZones();
            return _timeZones.FirstOrDefault(tz => tz.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        }

        static Task PopulateTimeZones()
        {

            if (_timeZones == null)
            {
                var timeZones = new List<TimeZoneInfo>();
                foreach (var z in TimeZoneInfo.GetSystemTimeZones())
                {
                    timeZones.Add(z);
                }
                _timeZones = new ReadOnlyCollection<TimeZoneInfo>(timeZones);
            }

            return Task.CompletedTask;

        }
    }

        


  
}
