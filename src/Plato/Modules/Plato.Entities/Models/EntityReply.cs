using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class EntityReply : IEntityReply
    {

        public int Id { get; set; }
        
        public int EntityId { get; set; }
        
        public int CategoryId { get; set; }
        
        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public int TotalReactions { get; set; }

        public int TotalReports { get; set; }
        
        public int MeanReactions { get; set; }
        
        public int MeanReports { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public SimpleUser CreatedBy { get; set; } = new SimpleUser();

        public SimpleUser ModifiedBy { get; set; } = new SimpleUser();
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
            
            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("Abstract"))
                Abstract = Convert.ToString(dr["Abstract"]);
       
            if (dr.ColumnIsNotNull("IsPrivate"))
                IsPrivate = Convert.ToBoolean(dr["IsPrivate"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("CreatedUserName"))
                    CreatedBy.UserName = Convert.ToString(dr["CreatedUserName"]);
                if (dr.ColumnIsNotNull("CreatedNormalizedUserName"))
                    CreatedBy.NormalizedUserName = Convert.ToString(dr["CreatedNormalizedUserName"]);
                if (dr.ColumnIsNotNull("CreatedDisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (ModifiedUserId > 0)
            {
                ModifiedBy.Id = ModifiedUserId;
                if (dr.ColumnIsNotNull("ModifiedUserName"))
                    ModifiedBy.UserName = Convert.ToString(dr["ModifiedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedNormalizedUserName"))
                    ModifiedBy.NormalizedUserName = Convert.ToString(dr["ModifiedNormalizedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedDisplayName"))
                    ModifiedBy.DisplayName = Convert.ToString(dr["ModifiedDisplayName"]);
            }

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

        }
        
    }
}
