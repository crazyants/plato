namespace Plato.Internal.Localization.Abstractions
{
    public class TimeZone : ITimeZone
    {

        public string Name { get; set; }

        public string Region { get; set; }

        public double UtcOffSet { get; set; }

        public TimeZone(string name, string region, double utcOffSet)
        {
            Name = name;
            Region = region;
            UtcOffSet = utcOffSet;
        }

    }

    public interface ITimeZone
    {
        string Name { get; set; }

        string Region { get; set; }

        double UtcOffSet { get; set; }
    }


}
