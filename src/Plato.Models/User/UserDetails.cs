using System.Collections.Generic;
using Plato.Models.Annotations;

namespace Plato.Models.User
{
    [TableName("Plato_UserDetails")]
    public class UserDetails
    {

        [ColumnName("Password")]
        public string Password { get; set; }

        [ColumnName("Salts")]
        public List<int> Salts { get; set; }
        
    }

}
