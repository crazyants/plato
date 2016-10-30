using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Abstractions.Extensions;
using Plato.Data.Query;
using Plato.Models.Users;

namespace Plato.Repositories.Users
{

    public enum QueryOperator
    {
        And,
        Or
    }

    public class QueryObjectString
    {
        private string _value;
        private QueryOperator _operator = QueryOperator.And;
        private readonly StringBuilder _builder;

        public QueryObjectString()
        {
            _builder = new StringBuilder();
        }
        
        public string Value => _value;

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public QueryObjectString Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }
        public QueryObjectString And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public QueryObjectString Equals(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            _value = value;
            _builder.Append("{0} = @{0}");
            return this;
        }

        public QueryObjectString StartsWith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            _value = value;
            _builder.Append("{0} LIKE '%' + @{0}");
            return this;
        }

        public QueryObjectString Endsith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            _value = value;
            _builder.Append("{0} LIKE @{0} + '%'");
            return this;
        }

        public QueryObjectString IsIn(string value, char delimiter = ',')
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            _value = value;
            _builder
                .Append("{0} IN ( SELECT * FROM plato_fn_ListToTable('")
                .Append(delimiter)
                .Append("', @{0})  )");
            return this;
        }

        public QueryObjectString Like(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            _value = value;
            _builder.Append("{0} LIKE '%' + @{0}");
            return this;
        }
        
        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
        }

    }

    public class UserQueryParams
    {
        private QueryObjectString _email;
        private QueryObjectString _userName;

        public int Id;
        public int[] Ids;

        public QueryObjectString UserName
        {
            get { return _userName ?? (_userName = new QueryObjectString()); }
            set { _userName = value; }
        }

        public QueryObjectString Email
        {
            get { return _email ?? (_email = new QueryObjectString()); }
            set { _email = value; }
        }

    }

    #region "UserQuery"

    public class UserQuery : DefaultQuery
    {
        private readonly IRepository<User> _repository;

        public UserQuery(IRepository<User> repository)
        {
            _repository = repository;
        }

        public UserQueryParams Params { get; set; }

        public override IQuery Define<T>(Action<T> action)
        {
            var defaultParams = new UserQueryParams();
            action((T)Convert.ChangeType(defaultParams, typeof(T)));
            Params = defaultParams;
            return this;
        }

        public override async Task<IEnumerable<T>> ToListAsync<T>()
        {
            IQueryBuilder builder = new UserQueryBuilder(this);

            // define all searchable input parameters 

            var id = Params.Id;
            var userName = Params.UserName;
            var email = Params.Email;

            return await _repository.SelectAsync<T>(
                builder.PageIndex,
                builder.PageSize,
                builder.BuildSqlStartId(),
                builder.BuildSqlPopulate(),
                builder.BuildSqlCount(),
                "@id int, @UserName nvarchar(255), @Email nvarchar(255)",
                id,
                userName != null ? userName.Value.ToEmptyIfNull() : string.Empty,
                email.Value
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
                sb.Append("Id >= @start_id_in");
            
            if (_query.Params.Id > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" AND ");
                sb.Append("Id = @Id");
            }

            if (!string.IsNullOrEmpty(_query.Params.UserName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserName.Operator);
                sb.Append(_query.Params.UserName.ToSqlString("UserName"));
            }

            if (!string.IsNullOrEmpty(_query.Params.Email.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Email.Operator);
                sb.Append(_query.Params.Email.ToSqlString("Email"));
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