using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace Plato.Internal.Models.Roles
{
    public class RoleClaim
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}
