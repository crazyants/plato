using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data.Query;
using System.Text;
using Plato.Data.Query;

namespace Plato.Repositories.Users
{
    public class UserQuery : DefaultQuery
    {

        private UserCriteria UserCriteria
        {
            get
            {
                UserCriteria criteria = this.Criteria as UserCriteria;
                if (criteria == null)
                    criteria = new UserCriteria();
                return criteria;
            }
        }

        public override string BuildSql()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM Plato_Roles");

            if (!string.IsNullOrEmpty(UserCriteria.Name))
            {
                sb.Append(("HERE Name = '"));
                sb.Append(UserCriteria.Name);
                sb.Append("'");
            }


            return sb.ToString();

        }

        public override string BuildSqlCount()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM Plato_Roles");

            return sb.ToString();
        }

    }

    public class UserCriteria : ICriteria
    {

        public string Name { get; set; }

        public bool IsAdministrator { get; set; }

    }



}
