using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Plato.Models.Users
{
    public class UserRole : IdentityUserRole<int>
    {

        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }
        
        public string ConcurrencyStamp { get; set; }

    }
}
