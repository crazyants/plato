using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Data.Query;
using Plato.Data;
using Plato.Repositories.Roles;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;

namespace Plato.Repositories.Users
{
    
    #region "UserQuery"

    public class UserQuery : DefaultQuery
    {
        private readonly IRepository<User> _repository;

        public UserQuery(IRepository<User> repository)
        {
            _repository = repository;
        }

        public override async Task<IEnumerable<T>> ToListAsync<T>() 
        {
            IQueryBuilder qr = new UserQueryBuilder(this);
            return await _repository.SelectAsync<T>(qr);
        }
        
    }

    #endregion

    #region "UserQueryBuilder"

    public class UserQueryBuilder : DefaultQueryBuilder
    {
        private readonly IQuery _query;

        public UserQueryBuilder(IQuery query)
        {
            _query = query;
        }

        public override string BuildSqlStartId()
        {
            var where = BuildWhere(false);
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id_out = Id FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }
        
        public override string BuildSqlPopulate()
        {
            var where = BuildWhere();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }

        public override string BuildSqlCount()
        {
            var where = BuildWhere();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }

        public override string BuildSqlParams()
        {

            //var expressions = _query.Expressions.ToList();
            //var sb = new StringBuilder();

            //var i = 0;
            //foreach (var exp in expressions)
            //{
            //    sb.Append("@");
            //    sb.Append(exp.Name);
            //    sb.Append(" sql_variant");
            //    if (i < expressions.Count - 1)
            //        sb.Append(", ");
            //    i += 1;
            //}

            //return sb.ToString();

            // @FirstId is required and represents the Id to start from
            return @"@Id int, @UserName nvarchar(255), @Email nvarchar(255)";

        }
        
        private string BuildWhere(bool includeStartId = true)
        {

            

            var sb = new StringBuilder();
            foreach (var exp in _query.Expressions)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append(exp.Name)
                .Append(exp.ExpressionOperator) 
                .Append("@").Append(exp.Name);
            }


            return sb.ToString();

            //return _query.ToString();

            //var sb = new StringBuilder();
        
            //if (includeStartId)
            //    sb.Append("Id >= @start_id_in");

            //if (!string.IsNullOrEmpty(UserCriteria.UserName))
            //{
            //    if (!string.IsNullOrEmpty(sb.ToString()))
            //        sb.Append(" AND ");
            //    sb.Append("UserName = @UserName");
            //}

            //if (!string.IsNullOrEmpty(UserCriteria.Email))
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
    
    #endregion


}