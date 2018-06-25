using System;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Users
{
    public class UserRole : IdentityUserRole<int>, IModel<UserRole>
    {
        public int Id { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("RoleId"))
                RoleId = Convert.ToInt32(dr["RoleId"]);

        }

    }
}