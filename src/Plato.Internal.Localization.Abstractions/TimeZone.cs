using System;

namespace Plato.Internal.Localization.Abstractions
{
    public class TimeZone : ITimeZone
    {

        public string Id { get; set; }

        public string DisplayName { get; set; }

        public TimeSpan UtcOffSet { get; set; }

        public TimeZone(string id, string displayName, TimeSpan utcOffSet)
        {
            Id = id;
            DisplayName = displayName;
            UtcOffSet = utcOffSet;
        }

    }

    public interface ITimeZone
    {
        string Id { get; set; }

        string DisplayName { get; set; }

        TimeSpan UtcOffSet { get; set; }
    }
    
}
