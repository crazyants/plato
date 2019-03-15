using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Roles
{

    public class Role : IdentityRole<int>, IDbModel<Role>
    {

        #region "Public Properties"
        
        public string Description { get; set;  }

        public List<RoleClaim> RoleClaims { get; } = new List<RoleClaim>();

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        #endregion

        #region "Constructors"

        public Role()
        {
        }

        public Role(string name)
            : base(name)
        {
            
        }

        public Role(int id, string name) : this(name)
        {
            base.Id = id;
        }
        
        #endregion

        #region "Implementation"

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
            
            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("NormalizedName"))
                NormalizedName = Convert.ToString(dr["NormalizedName"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);
            
            if (dr.ColumnIsNotNull("Claims"))
            {
                var claims = Convert.ToString(dr["Claims"]).Deserialize<List<RoleClaim>>();
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        this.RoleClaims.Add(claim);
                    }
                }
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ConcurrencyStamp"))
                ConcurrencyStamp = Convert.ToString(dr["ConcurrencyStamp"]);
            
        }

        #endregion

    }
}