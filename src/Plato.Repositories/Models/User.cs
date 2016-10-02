using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Plato.Repositories.Models
{
    public class User : IdentityUser<int>, IModel<User>
    {

        #region "Public Properties"

        public int ID { get; set;  }

        public string Name { get; set;  }

        public string EmailAddress { get; set; }

        public int TennetID { get; set; }

        public int SiteID { get; set; }

        #endregion

        public void PopulateModelFromDataReader(IDataReader dr)
        {

            if ((!object.ReferenceEquals((object)dr["UserID"], System.DBNull.Value)) & ((object)dr["UserID"] != null))
            {
                this.ID = Convert.ToInt32(dr["UserID"]);
            }

            if ((!object.ReferenceEquals((object)dr["Username"], System.DBNull.Value)) & ((object)dr["Username"] != null))
            {
                this.Name = Convert.ToString(dr["Username"]);
            }

            if ((!object.ReferenceEquals((object)dr["EmailAddress"], System.DBNull.Value)) & ((object)dr["EmailAddress"] != null))
            {
                this.EmailAddress = Convert.ToString(dr["EmailAddress"]);
            }

        }
    }

       
}
