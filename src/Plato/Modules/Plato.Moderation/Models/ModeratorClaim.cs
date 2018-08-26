using System.Security.Claims;
using Newtonsoft.Json;

namespace Plato.Moderation.Models
{
 
    public class ModeratorClaim
    {
      
        public string ClaimType { get; set; }
        
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
