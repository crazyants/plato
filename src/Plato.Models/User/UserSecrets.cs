using System.Collections.Generic;
using Plato.Models.Annotations;

namespace Plato.Models.User
{

    [TableName("Plato_UserSecrets")]
    public class UserSecrets 
    {

        [PrimaryKey]
        [ColumnName("Id", typeof(int))]
        public int ID { get; set; }

        [ColumnName("UserId", typeof(int))]
        public int UserId { get; set; }
        
        [ColumnName("Password", typeof(string), 255)]
        public string Password { get; set;  }

        [ColumnName("Salts", typeof(List<int>))]
        public List<int> Salts { get; set;  }

    }

}
