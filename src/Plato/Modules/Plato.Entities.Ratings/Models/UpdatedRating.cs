using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Entities.Ratings.Models
{
    public class UpdatedRating
    {
        public int TotalRatings { get; set; }

        public int MeanRating { get; set; }

        public double DailyRatings { get; set; }
    }

}
