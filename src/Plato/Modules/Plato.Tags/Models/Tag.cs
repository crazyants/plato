using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Tags.Models
{
    
    public class Tag : IModel<Tag>, ITag
    {

        public int Id { get; set; }

        public int FeatureId { get; set; }
        
        public string Name { get; set; }

        public string Alias { get; set; }

        public int TotalEntities { get; set; }

        public int TotalFollows { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);
            
            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("TotalEntities"))
                TotalEntities = Convert.ToInt32(dr["TotalEntities"]);

            if (dr.ColumnIsNotNull("TotalFollows"))
                TotalFollows = Convert.ToInt32(dr["TotalFollows"]);
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

        }

    }

}
