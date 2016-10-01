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

        public int TennetID { get; set; }

        public int SiteID { get; set; }

        public void PopulateModelFromDataReader(IDataReader dr)
        {

            if ((!object.ReferenceEquals((object)dr["TennetID"], System.DBNull.Value)) & ((object)dr["TennetID"] != null))
            {

                this.TennetID = Convert.ToInt32(dr["TennetID"]);
            }

        }
    }

       
}
