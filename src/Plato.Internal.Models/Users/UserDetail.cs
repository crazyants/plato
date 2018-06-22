using System;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Users
{
    
    public class UserDetail : ISerializable
    {

        #region "Public Properties"
        
        public bool IsEmailConfirmed { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        #endregion

        #region "Implementation"

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }



        #endregion#

    }

}
