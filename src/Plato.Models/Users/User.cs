using System;
using System.Collections;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Plato.Abstractions.Extensions;
using Plato.Models.Annotations;
using Plato.Models.Roles;
using System.Collections.Generic;

namespace Plato.Models.Users
{

    [TableName("Plato_Users")]
    public class User : IdentityUser<int>, IModel<User>
    {

        #region "Public Properties"
                   
        [ColumnName("DisplayName", typeof(string), 255)]
        public string DisplayName { get; set; }
        
        [ColumnName("SamAccountName", typeof(string), 255)]
        public string SamAccountName { get; set;  }
          
        public UserSecret Secret { get; set;  }

        public UserDetail Detail { get; set; }

        public UserPhoto Photo { get; set; }

        public IEnumerable<Role> UserRoles { get; set; }
        
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

            if (dr.ColumnIsNotNull("id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserName"))
                this.UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("NormalizedUserName"))
                this.NormalizedUserName = Convert.ToString(dr["NormalizedUserName"]);
            
            if (dr.ColumnIsNotNull("Email"))
                this.Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("NormalizedEmail"))
                this.NormalizedEmail = Convert.ToString(dr["NormalizedEmail"]);
            
            if (dr.ColumnIsNotNull("DisplayName"))            
                this.DisplayName = Convert.ToString(dr["DisplayName"]);

            if (dr.ColumnIsNotNull("SamAccountName"))
                this.SamAccountName = Convert.ToString(dr["SamAccountName"]);

        }

        public void PopulateModel(Action<User> action)
        {
            action(this);
        }

        #endregion

    }
       
}
