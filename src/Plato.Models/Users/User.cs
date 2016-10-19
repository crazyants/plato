using System;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Plato.Abstractions.Extensions;
using Plato.Models.Annotations;

namespace Plato.Models.Users
{

    [TableName("Plato_Users")]
    public class User : IdentityUser<int>, IModel<User>
    {

        #region "Public Properties"
              
        [ColumnName("SiteId", typeof(int))]
        public int SiteId { get; set; }

        [ColumnName("DisplayName", typeof(string), 255)]
        public string DisplayName { get; set; }
        
        [ColumnName("SamAccountName", typeof(string), 255)]
        public string SamAccountName { get; set;  }
          
        public UserSecret Secret { get; set;  }

        public UserDetail Detail { get; set; }

        public UserPhoto Photo { get; set; }
        
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

            if (dr.ColumnIsNotNull("Email"))
                this.Email = Convert.ToString(dr["Email"]);
                  
            if (dr.ColumnIsNotNull("DisplayName"))            
                this.DisplayName = Convert.ToString(dr["DisplayName"]);
            
        }

        #endregion        

    }
       
}
