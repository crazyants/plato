using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Roles;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Plato.Internal.Models.Users
{
    
    public class User : IdentityUser<int>, IModel<User>
    {

        private string _displayName;

        #region "Public Properties"

        public string DisplayName
        {
            get => string.IsNullOrWhiteSpace(_displayName) ? this.UserName : _displayName;
            set => _displayName = value;
        }

        public int PrimaryRoleId { get; set; }

        public string SamAccountName { get; set;  }
          
        public string ApiKey { get; set; }

        public UserSecret Secret { get; set;  }

        public UserDetail Detail { get; set; }
        
        public IEnumerable<string> RoleNames { get; set; } = new List<string>();

        public IEnumerable<Role> UserRoles { get; } = new List<Role>();
        
        public IEnumerable<UserData> Data { get; set; } = new List<UserData>();

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
            
            if (dr.ColumnIsNotNull("ApiKey"))
                this.ApiKey = Convert.ToString(dr["ApiKey"]);

        }

        #endregion

    }
       
}
