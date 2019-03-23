namespace Plato.Entities.Ratings.Models
{
    public class AggregateRating
    {
        public int TotalRatings { get; set; }

        public int SummedRating { get; set; }

        public int MeanRating { get; set; }

        public double DailyRatings { get; set; }

    }

}
