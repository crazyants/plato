using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

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

        public TType ConvertToType<TType>() where TType : class, ITag
        {
            var tag = ActivateInstanceOf<TType>.Instance();
            tag.Id = TagId;
            tag.FeatureId = FeatureId;
            tag.Name = Name;
            tag.NameNormalized = NameNormalized;
            tag.Description = Description;
            tag.Alias = Alias;
            tag.TotalEntities = TotalEntities;
            tag.TotalFollows = TotalFollows;
            tag.LastSeenDate = LastSeenDate;
            tag.CreatedUserId = CreatedUserId;
            tag.CreatedDate = CreatedDate;
            return tag;
        }

    }

}
