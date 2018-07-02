using System;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Entities.Models
{
    public class EntityReply : EntityBase
    {

        public int EntityId { get; set; }

        public override void PopulateModel(IDataReader dr)
        {
           
            base.PopulateModel(dr);
            
            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

        }


    }
}
