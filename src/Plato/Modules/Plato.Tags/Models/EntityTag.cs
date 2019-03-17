using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Tags;

namespace Plato.Tags.Models
{

    public class EntityTag : TagBase, IDbModel<EntityTag>
    {

        public int EntityId { get; set; }

        public int EntityReplyId { get; set; }

        public int TagId { get; set; }
        
        public override void PopulateModel(IDataReader dr)
        {
            
            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("EntityReplyId"))
                EntityReplyId = Convert.ToInt32(dr["EntityReplyId"]);
            
            if (dr.ColumnIsNotNull("TagId"))
                TagId = Convert.ToInt32(dr["TagId"]);

            base.PopulateModel(dr);

        }

    }

}
