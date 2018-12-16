using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Follows.Models
{
    public class Follow : IModel<Follow>
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int ThingId { get; set; }
        
        public string CancellationToken { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public SimpleUser User { get; set; } = new SimpleUser();

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);
            
            if (dr.ColumnIsNotNull("ThingId"))
                ThingId = Convert.ToInt32(dr["ThingId"]);
            
            if (dr.ColumnIsNotNull("CancellationToken"))
                CancellationToken = Convert.ToString(dr["CancellationToken"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                User.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("UserName"))
                    User.UserName = Convert.ToString(dr["UserName"]);

                if (dr.ColumnIsNotNull("DisplayName"))
                    User.DisplayName = Convert.ToString(dr["DisplayName"]);

            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

        }
    }
}
