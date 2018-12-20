using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.History.Models
{
    public class EntityHistory : IModel<EntityHistory>
    {

        public int Id { get; set; }

        public int EntityId { get; set; }

        public int EntityReplyId { get; set; }
        
        public string Message { get; set; }

        public short MajorVersion { get; set; }

        public short MinorVersion { get; set; }
        
        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public SimpleUser User { get; set; } = new SimpleUser();
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("EntityReplyId"))
                EntityReplyId = Convert.ToInt32(dr["EntityReplyId"]);

            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("MajorVersion"))
                MajorVersion = Convert.ToInt16(dr["MajorVersion"]);
            
            if (dr.ColumnIsNotNull("MinorVersion"))
                MinorVersion = Convert.ToInt16(dr["MinorVersion"]);
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                User.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("UserName"))
                    User.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    User.DisplayName = Convert.ToString(dr["DisplayName"]);
                if (dr.ColumnIsNotNull("FirstName"))
                    User.FirstName = Convert.ToString(dr["FirstName"]);
                if (dr.ColumnIsNotNull("LastName"))
                    User.LastName = Convert.ToString(dr["LastName"]);
                if (dr.ColumnIsNotNull("Alias"))
                    User.Alias = Convert.ToString(dr["Alias"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));
            
        }

    }

}
