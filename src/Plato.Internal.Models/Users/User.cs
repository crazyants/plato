using System;
using System.Data;
using Plato.Abstractions.Extensions;
using Plato.Internal.Models.Annotations;
using Plato.Internal.Models.Roles;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Plato.Internal.Models.Users
{

    [TableName("Users")]
    public class User : IdentityUser<int>, IModel<User>
    {

        #region "Public Properties"
                   
        [ColumnName("DisplayName", typeof(string), 255)]
        public string DisplayName { get; set; }
        
        [ColumnName("SamAccountName", typeof(string), 255)]
        public string SamAccountName { get; set;  }
          
        public UserSecret Secret { get; set;  }

        public UserDetail Detail { get; set; }
        
        public List<string> RoleNames { get; set; } = new List<string>();

        public List<Role> UserRoles { get; } = new List<Role>();
        
        #endregion

        #region "constructor"

        public User()
        {
            this.Detail = new UserDetail();
            this.Secret = new UserSecret();
        }

        #endregion
        
        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserName"))
                this.UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("NormalizedUserName"))
                this.NormalizedUserName = Convert.ToString(dr["NormalizedUserName"]);
            
            if (dr.ColumnIsNotNull("Email"))
                this.Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("NormalizedEmail"))
                this.NormalizedEmail = Convert.ToString(dr["NormalizedEmail"]);
            
            if (dr.ColumnIsNotNull("EmailConfirmed"))
                this.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);

            if (dr.ColumnIsNotNull("DisplayName"))            
                this.DisplayName = Convert.ToString(dr["DisplayName"]);

            if (dr.ColumnIsNotNull("SamAccountName"))
                this.SamAccountName = Convert.ToString(dr["SamAccountName"]);

            if (dr.ColumnIsNotNull("PasswordHash"))
                this.PasswordHash = Convert.ToString(dr["PasswordHash"]);

            if (dr.ColumnIsNotNull("SecurityStamp"))
                this.SecurityStamp = Convert.ToString(dr["SecurityStamp"]);

            if (dr.ColumnIsNotNull("PhoneNumber"))
                this.PhoneNumber = Convert.ToString(dr["PhoneNumber"]);

            if (dr.ColumnIsNotNull("PhoneNumberConfirmed"))
                this.PhoneNumberConfirmed = Convert.ToBoolean(dr["PhoneNumberConfirmed"]);

            if (dr.ColumnIsNotNull("TwoFactorEnabled"))
                this.TwoFactorEnabled = Convert.ToBoolean(dr["TwoFactorEnabled"]);

            if (dr.ColumnIsNotNull("LockoutEnd"))
                this.LockoutEnd = Convert.ToDateTime(dr["LockoutEnd"]);

            if (dr.ColumnIsNotNull("LockoutEnabled"))
                this.LockoutEnabled = Convert.ToBoolean(dr["LockoutEnabled"]);

            if (dr.ColumnIsNotNull("AccessFailedCount"))
                this.AccessFailedCount = Convert.ToInt32(dr["AccessFailedCount"]);

        }
        
        #endregion

    }
       
}
