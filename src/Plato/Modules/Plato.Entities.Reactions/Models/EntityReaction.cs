using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Reactions.Models
{
    
    public class EntityReaction : IModel<EntityReaction>
    {

        public int Id { get; set; }
   
        public string ReactionName { get; set; }

        public Sentiment Sentiment { get; set; }

        public int Points { get; set; }

        public int EntityId { get; set; }

        public int EntityReplyId { get; set; }
        
        public int CreatedUserId { get; set; }
        
        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
          
            if (dr.ColumnIsNotNull("ReactionName"))
                ReactionName = Convert.ToString(dr["ReactionName"]);

            if (dr.ColumnIsNotNull("Sentiment"))
                Sentiment = (Sentiment) Convert.ToInt16(dr["Sentiment"]);

            if (dr.ColumnIsNotNull("Points"))
                Points = Convert.ToInt32(dr["Points"]);
            
            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("EntityReplyId"))
                EntityReplyId = Convert.ToInt32(dr["EntityReplyId"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            { 
                CreatedBy = new SimpleUser()
                {
                    Id = CreatedUserId
                };
                if (dr.ColumnIsNotNull("UserName"))
                    CreatedBy.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["DisplayName"]);
                if (dr.ColumnIsNotNull("Alias"))
                    CreatedBy.Alias = Convert.ToString(dr["Alias"]);
                if (dr.ColumnIsNotNull("PhotoUrl"))
                    CreatedBy.PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
                if (dr.ColumnIsNotNull("PhotoColor"))
                    CreatedBy.PhotoColor = Convert.ToString(dr["PhotoColor"]);
            }
            
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
