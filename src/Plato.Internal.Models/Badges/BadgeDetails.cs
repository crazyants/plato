using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Badges
{

    public interface IBadgeDetails
    {
        string BadgeName { get; set; }

        int AwardedCount { get; set; }

        DateTimeOffset? FirstAwardedDate { get; set; }

        DateTimeOffset? LastAwardedDate { get; set; }

    }

    public class BadgeDetails : IBadgeDetails, IDbModel
    {

        public string BadgeName { get; set; }

        public int AwardedCount { get; set; }

        public DateTimeOffset? FirstAwardedDate { get; set; }

        public DateTimeOffset? LastAwardedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("BadgeName"))
                BadgeName = Convert.ToString(dr["BadgeName"]);

            if (dr.ColumnIsNotNull("AwardedCount"))
                AwardedCount = Convert.ToInt32(dr["AwardedCount"]);

            if (dr.ColumnIsNotNull("FirstAwardedDate"))
                FirstAwardedDate = (DateTimeOffset)dr["FirstAwardedDate"];

            if (dr.ColumnIsNotNull("LastAwardedDate"))
                LastAwardedDate = (DateTimeOffset)dr["LastAwardedDate"];

        }
    }

}
