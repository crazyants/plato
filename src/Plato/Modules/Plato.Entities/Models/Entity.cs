using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Entities.Models
{
    public class Entity : EntityBase
    {

        public Entity() :base()
        {
            
        }

        public override void PopulateModel(IDataReader dr)
        {

            base.PopulateModel(dr);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);
            
            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("TotalViews"))
                TotalViews = Convert.ToInt32(dr["TotalViews"]);
            
            if (dr.ColumnIsNotNull("TotalReplies"))
                TotalReplies = Convert.ToInt32(dr["TotalReplies"]);

            if (dr.ColumnIsNotNull("TotalFollows"))
                TotalFollows = Convert.ToInt32(dr["TotalFollows"]);

        }

    }
    
}
