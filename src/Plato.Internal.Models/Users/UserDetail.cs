using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Annotations;
using Plato.Internal.Abstractions.Extensions;

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
            throw new NotImplementedException();
        }

        #endregion#

    }

}
