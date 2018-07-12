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

            if (dr.ColumnIsNotNull("TitleNormalized"))
                TitleNormalized = Convert.ToString(dr["TitleNormalized"]);

        }

    }
    
}
