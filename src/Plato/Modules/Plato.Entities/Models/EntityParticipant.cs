using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public class EntityParticipant : IModel<EntityParticipant>
    {

        public int Id { get; set; }

        public int EntityId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? CreatedDate { get; set; }


        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);
            
            if (dr.ColumnIsNotNull("UserName"))
                UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

        }
    }
}
