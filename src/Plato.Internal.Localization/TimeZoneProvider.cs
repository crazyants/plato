using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization
{

    public class TimeZoneProvider : ITimeZoneProvider
    {
        private static ReadOnlyCollection<TimeZoneInfo> _timeZones;
   
        public Task<ReadOnlyCollection<TimeZoneInfo>> GetTimeZonesAsync()
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

            return Task.FromResult(_timeZones);

        }

    }


  
}
