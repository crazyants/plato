using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;
using Plato.Internal.Models.Users;

namespace Plato.Follow.Models
{
    public class EntityFollow : IModel<EntityFollow>
    {

        public int Id { get; set; }

        public int EntityId { get; set; }

        public int UserId { get; set; }

        public string CancellationGuid { get; set; }

        public DateTime CreatedDate { get; set; }

        public SimpleUser User { get; set; } = new SimpleUser();

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);

            if (UserId > 0)
            {
                User.Id = UserId;

                if (dr.ColumnIsNotNull("UserName"))
                    User.UserName = Convert.ToString(dr["UserName"]);

                if (dr.ColumnIsNotNull("Email"))
                    User.Email = Convert.ToString(dr["Email"]);

                if (dr.ColumnIsNotNull("DisplayName"))
                    User.DisplayName = Convert.ToString(dr["DisplayName"]);

                if (dr.ColumnIsNotNull("NormalizedUserName"))
                    User.NormalizedUserName = Convert.ToString(dr["NormalizedUserName"]);

            }

            if (dr.ColumnIsNotNull("CancellationGuid"))
                CancellationGuid = Convert.ToString(dr["CancellationGuid"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

        }
    }
}
