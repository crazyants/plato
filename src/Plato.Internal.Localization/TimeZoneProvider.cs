using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Internal.Localization.Abstractions;

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
        
        public Task<IEnumerable<TTimeZone>> GetTimeZones()
        {

            if (_timeZones == null)
            {
                _timeZones = new List<TimeZone>()
                {
                    new TimeZone(S["(GMT -12:00)"], S["Eniwetok, Kwajalein"], -12),
                    new TimeZone(S["(GMT -11:00)"], S["Midway Island"], -11),
                    new TimeZone(S["(GMT -10:00)"], S["Hawaii"], -10),
                    new TimeZone(S["(GMT -9:00)"], S["Alaska"], -9),
                    new TimeZone(S["(GMT -8:00)"], S["Pacific Time"], -8),
                    new TimeZone(S["(GMT -7:00)"], S["Mountain Time"], -7),
                    new TimeZone(S["(GMT -6:00)"], S["Central Time"], -6),
                    new TimeZone(S["(GMT -5:00)"], S["Eastern Time"], -5),
                    new TimeZone(S["(GMT -4:00)"], S["Atlantic Time"], -4),
                    new TimeZone(S["(GMT -3:30)"], S["Newfoundland"], -3.5),
                    new TimeZone(S["(GMT -3:00)"], S["Brazil"], -3),
                    new TimeZone(S["(GMT -2:00)"], S["Mid-Atlantic"], -2),
                    new TimeZone(S["(GMT -1:00)"], S["Cape Verde Islands"], -1),
                    new TimeZone(S["(GMT)"], S["Western European"], 0),
                    new TimeZone(S["(GMT) +1:00"], S["Brussels, Paris"], 1),
                    new TimeZone(S["(GMT) +2:00"], S["South Africa"], 2),
                    new TimeZone(S["(GMT) +3:00"], S["Baghdad, Moscow"], 3),
                    new TimeZone(S["(GMT) +3:30"], S["Baghdad, Tehran"], 3.5),
                    new TimeZone(S["(GMT) +4:00"], S["Abu Dhabi, Baku"], 4),
                    new TimeZone(S["(GMT) +4:30"], S["Kabul"], 4.5),
                    new TimeZone(S["(GMT) +5:00"], S["Karachi"], 5),
                    new TimeZone(S["(GMT) +5:30"], S["Bombay, Calcutta"], 5.5),
                    new TimeZone(S["(GMT) +6:00"], S["Almaty, Dhaka"], 6),
                    new TimeZone(S["(GMT) +7:00"], S["Bangkok, Hanoi"], 7),
                    new TimeZone(S["(GMT) +8:00"], S["Beijing, Singapore"], 8),
                    new TimeZone(S["(GMT) +9:00"], S["Tokyo, Seoul"], 9),
                    new TimeZone(S["(GMT) +9:30"], S["Darwin"], 9.5),
                    new TimeZone(S["(GMT) +10:00"], S["Eastern Australia"], 10),
                    new TimeZone(S["(GMT) +11:00"], S["New Caledonia"], 11),
                    new TimeZone(S["(GMT) +12:00"], S["Auckland"], 12)
                };
            }

            return Task.FromResult((IEnumerable<TTimeZone>) _timeZones);

        }

    }


  
}
