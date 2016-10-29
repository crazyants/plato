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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Plato.Data.Query;
using Plato.Data;
using Plato.Repositories.Roles;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;

namespace Plato.Repositories.Users
{

    public class UserQueryObject 
    {
        public int Id;
        public int[] Ids;
        public string UserNameEquals;
        public string UserNameStartsWith;
        public string UserNameEndsWith;
        public string UserNameContains;
        public string Email;
    }

    #region "UserQuery"

    public class UserQuery : DefaultQuery
    {
        private readonly IRepository<User> _repository;

        public UserQuery(IRepository<User> repository)
        {
            _repository = repository;
        }

        public UserQueryObject Paramaters { get; set; }

        public override IQuery Define<T>(Action<T> action)
        {
            var defaultQueryObject = new UserQueryObject();
            action((T)Convert.ChangeType(defaultQueryObject, typeof(T)));
            this.Paramaters = defaultQueryObject;
            return this;
        }

        public override async Task<IEnumerable<T>> ToListAsync<T>() 
        {
            
            IQueryBuilder builder = new UserQueryBuilder(this);

            // define all searchable input parameters 

            var id = this.Paramaters.Id;
            var userName = this.Paramaters.UserNameEquals;
            var email = this.Paramaters.Email;

            if (string.IsNullOrEmpty(userName))
                userName = this.Paramaters.UserNameStartsWith;

            if (string.IsNullOrEmpty(userName))
                userName = this.Paramaters.UserNameEndsWith;

            if (string.IsNullOrEmpty(userName))
                userName = this.Paramaters.UserNameContains;


            return await _repository.SelectAsync<T>(
                builder.PageIndex,
                builder.PageSize,
                builder.BuildSqlStartId(),
                builder.BuildSqlPopulate(),
                builder.BuildSqlCount(),
                "@id int, @UserName nvarchar(255), @Email nvarchar(255)",
                id,
                userName.ToEmptyIfNull(),
                email.ToEmptyIfNull()
                );

        }
        
    }

    #endregion

    #region "UserQueryBuilder"

    public class UserQueryBuilder : DefaultQueryBuilder
    {

        private readonly UserQuery _query;
   
        public UserQueryBuilder(UserQuery query)
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
        
        private string BuildWhere(bool includeStartId = true)
        {
            
            var sb = new StringBuilder();
            if (includeStartId)
                sb.Append(("Id >= @start_id_in"));

        
            if (_query.Paramaters.Id > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("Id = @Id");
            }
            
            if (!string.IsNullOrEmpty(_query.Paramaters.UserNameEquals))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("UserName = @UserName");
            }

            if (!string.IsNullOrEmpty(_query.Paramaters.UserNameStartsWith))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("UserName LIKE '%' + @UserName");
            }

            if (!string.IsNullOrEmpty(_query.Paramaters.UserNameEndsWith))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("UserName LIKE @UserName + '%'");
            }

            if (!string.IsNullOrEmpty(_query.Paramaters.UserNameContains))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("UserName LIKE '%' + @UserName + '%'");
            }


            return sb.ToString();

        }

        private string BuildOrderBy()
        {

            return "";

        }

    }
    
    #endregion


}