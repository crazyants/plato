using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Internal.Localization.Abstractions;
using TimeZone = Plato.Internal.Localization.Abstractions.TimeZone;

namespace Plato.Internal.Localization
{
    
    public class TimeZoneProvider<TTimeZone> : ITimeZoneProvider<TTimeZone> where TTimeZone : class, ITimeZone
    {

        private static IEnumerable<TimeZone> _timeZones;

        public IStringLocalizer S { get; }

        public TimeZoneProvider(
            IStringLocalizer<TimeZoneProvider<TTimeZone>> stringLocalizer)
        {
            S = stringLocalizer;
      
        }
        
        public Task<IEnumerable<TTimeZone>> GetTimeZonesAsync()
        {

            if (_timeZones == null)
            {
                var timeZones = new List<TimeZone>();
                foreach (var z in TimeZoneInfo.GetSystemTimeZones())
                {
                    timeZones.Add(new TimeZone(z.Id, z.DisplayName, z.BaseUtcOffset));
                }
                _timeZones = timeZones;
            }

            return Task.FromResult((IEnumerable<TTimeZone>) _timeZones);

        }

    }


  
}
