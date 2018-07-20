using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public class EntityCategory : IModel<EntityCategory>
    {

        public int Id { get; set; }

        public int EntityId { get; set; }

        public int CategoryId { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("CategoryId"))
                CategoryId = Convert.ToInt32(dr["CategoryId"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

        }

    }
}
