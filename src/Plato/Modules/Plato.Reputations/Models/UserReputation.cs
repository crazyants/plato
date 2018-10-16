using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Reputations.Models
{
    public class UserReputation : IModel<UserReputation>
    {

        public int Id { get; set; }

        public string ReputationName { get; set; }

        public int UserId { get; set; }

        public int Points { get; set; }
        
        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ReputationName"))
                ReputationName = Convert.ToString(dr["ReputationName"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("Points"))
                Points = Convert.ToInt32(dr["Points"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

        }
        
    }

}
