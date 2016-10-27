using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data.Query;
using System.Text;

namespace Plato.Repositories.Roles
{
    public class RoleQuery : DefaultQuery
    {

        private RoleCriteria RoleCriteria
        {
            get
            {
                RoleCriteria criteria = this.Criteria as RoleCriteria;
                if (criteria == null)
                    criteria = new RoleCriteria();
                return criteria;
            }
        }

        public override string BuildSql()
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM Plato_Roles");

            if (!string.IsNullOrEmpty(RoleCriteria.Name))
            {
                sb.Append(("HERE Name = '"));
                sb.Append(RoleCriteria.Name);
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

    public class RoleCriteria : ICriteria
    {

        public string Name { get; set; }

        public bool IsAdministrator { get; set; }
        
    }



}
