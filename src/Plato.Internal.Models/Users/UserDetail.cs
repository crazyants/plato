using System;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Users
{
    
    public class UserDetail : Serializable
    {

        #region "Public Properties"
        
        public bool IsEmailConfirmed { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        #endregion

        #region "Implementation"
        
        #endregion#

    }

}
