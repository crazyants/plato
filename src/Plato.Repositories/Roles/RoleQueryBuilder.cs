using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data.Query;
using System.Text;

namespace Plato.Repositories.Roles
{
    public class RoleQueryBuilder : DefaultQueryBuilder
    {
        private readonly IQuery _query;

        public RoleQueryBuilder(IQuery query)
        {
            _query = query;
        }

        public override string BuildSqlStartId()
        {

            var where = Buildwhere();
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id = Id FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ").Append(where);

            return sb.ToString();


        }

        public override string BuildSqlPopulate()
        {

            var where = Buildwhere();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ").Append(where);
          
            return sb.ToString();

        }


        public override string BuildSqlCount()
        {
            var where = Buildwhere();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ").Append(where);

            return sb.ToString();
        }

        public override string BuildSqlParams()
        {
            throw new NotImplementedException();

        }

        private string Buildwhere()
        {

            return _query.ToString();

            //var sb = new StringBuilder();
            //if (!string.IsNullOrEmpty(RoleCriteria.UserName))
            //{
            //    if (!string.IsNullOrEmpty(sb.ToString()))
            //        sb.Append(" AND ");
            //    sb.Append("UserName = @UserName");
            //}

            //if (!string.IsNullOrEmpty(RoleCriteria.UserName))
            //{
            //    if (!string.IsNullOrEmpty(sb.ToString()))
            //        sb.Append(" AND ");
            //    sb.Append("Email = @Email");
            //}


            //return sb.ToString();
        }

        private string BuildOrderBy()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

    }
    

}
