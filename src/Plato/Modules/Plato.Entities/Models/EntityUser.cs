using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{
    public class EntityUser : SimpleUser, IDbModel<EntityUser>
    {

        public int LastReplyId { get; set; }

        public DateTimeOffset? LastReplyDate { get; set; }

        public int TotalReplies { get; set; }

        public EntityUser() 
        {
        }

        public EntityUser(IUser user) : base(user)
        {
        }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("UserId"))
                Id = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("UserName"))
                UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("DisplayName"))
                DisplayName = Convert.ToString(dr["DisplayName"]);
            
            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);
            
            if (dr.ColumnIsNotNull("PhotoUrl"))
                PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
            
            if (dr.ColumnIsNotNull("PhotoColor"))
                PhotoColor = Convert.ToString(dr["PhotoColor"]);
            
            if (dr.ColumnIsNotNull("LastReplyId"))
                LastReplyId = Convert.ToInt32(dr["LastReplyId"]);

            if (dr.ColumnIsNotNull("LastReplyDate"))
                LastReplyDate = (DateTimeOffset)dr["LastReplyDate"];

            if (dr.ColumnIsNotNull("TotalReplies"))
                TotalReplies = Convert.ToInt32(dr["TotalReplies"]);

        }

    }

}
