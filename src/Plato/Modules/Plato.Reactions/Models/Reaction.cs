using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Reactions.Models
{
    public class Reaction : IModel<Reaction>
    {
        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Emoji { get; set; }

        public bool IsPositive { get; set; }

        public bool IsNeutral { get; set; }

        public bool IsNegative { get; set; }

        public bool IsDisabled { get; set; }
        
        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);
            
            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("Emoji"))
                Emoji = Convert.ToString(dr["Emoji"]);

            if (dr.ColumnIsNotNull("IsPositive"))
                IsPositive = Convert.ToBoolean(dr["IsPositive"]);

            if (dr.ColumnIsNotNull("IsNeutral"))
                IsNeutral = Convert.ToBoolean(dr["IsNeutral"]);

            if (dr.ColumnIsNotNull("IsNegative"))
                IsNegative = Convert.ToBoolean(dr["IsNegative"]);

            if (dr.ColumnIsNotNull("IsDisabled"))
                IsDisabled = Convert.ToBoolean(dr["IsDisabled"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

        }
    }
}
