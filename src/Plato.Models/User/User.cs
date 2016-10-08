using System;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Plato.Abstractions.Extensions;
using Plato.Models.Annotations;
using System.Collections.Generic;

namespace Plato.Models.User
{

    [TableName("Plato_User")]
    public class User : IdentityUser<int>, IModel<User>
    {

        #region "Public Properties"

        [PrimaryKey]
        [ColumnName("Id", typeof(int))]
        public int ID { get; set;  }

        [ColumnName("TennetId", typeof(int))]
        public int TennetId { get; set;  }

        [ColumnName("SiteId", typeof(int))]
        public int SiteId { get; set; }

        [ColumnName("Name", typeof(string), 255)]
        public string Name { get; set;  }

        [ColumnName("EmailAddress", typeof(string), 255)]
        public string EmailAddress { get; set; }

        [ColumnName("SamAccountName", typeof(string), 255)]
        public string SamAccountName { get; set;  }
        
        public UserSecrets Secrets { get; set;  }

        public UserDetails Details { get; set; }
        
        #endregion

        #region "Implementation"

        public void PopulateModelFromDataReader(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                this.ID = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EmailAddress"))
                this.EmailAddress = Convert.ToString(dr["EmailAddress"]);
            
            if (dr.ColumnIsNotNull("Username"))            
                this.Name = Convert.ToString(dr["Username"]);
            
            if (dr.ColumnIsNotNull("EmailAddress"))            
                this.EmailAddress = Convert.ToString(dr["EmailAddress"]);
            
            PopulateSecrets(dr);


        }

        #endregion

        #region "Private Methods"

        private void PopulateSecrets(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Password"))
            {
                this.Secrets = new UserSecrets();
                this.Secrets.Password = Convert.ToString(dr["Password"]);
                if (dr.ColumnIsNotNull("Salts"))
                {
                    this.Secrets.Salts = new List<int>();
                    string salts = Convert.ToString(dr["Salts"]);
                    if (!string.IsNullOrEmpty(salts))
                    {
                        var splitSalts = salts.Split(',');
                        foreach (string salt in splitSalts)
                        {
                            int saltOut;
                            bool ok = Int32.TryParse(salt, out saltOut);
                            if (ok)
                                this.Secrets.Salts.Add(saltOut);
                        }
                    }
                }
            }

        }

        #endregion

    }
       
}
