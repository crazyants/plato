using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Stars.Models
{
    public class Star : IDbModel<Star>
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int ThingId { get; set; }
        
        public string CancellationToken { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public IUser CreatedBy { get; set; } = new User();

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
                CreatedBy.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("UserName"))
                    CreatedBy.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("Email"))
                    CreatedBy.Email = Convert.ToString(dr["Email"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["DisplayName"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }
    }
}
