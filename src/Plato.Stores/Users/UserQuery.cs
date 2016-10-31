using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Abstractions.Stores;
using Plato.Stores.Query;

namespace Plato.Stores.Users
{
    public class UserQueryParams
    {
        private WhereString _email;

        public WhereInt _id;

        private WhereString _userName;

        public WhereInt Id
        {
            get { return _id ?? (_id = new WhereInt()); }
            set { _id = value; }
        }

        public WhereString UserName
        {
            get { return _userName ?? (_userName = new WhereString()); }
            set { _userName = value; }
        }

        public WhereString Email
        {
            get { return _email ?? (_email = new WhereString()); }
            set { _email = value; }
        }
    }

    public class UserQuery : DefaultQuery
    {
        private readonly IStore<User> _store;

        public UserQuery(IStore<User> store)
        {
            _store = store;
        }

        public UserQueryParams Params { get; set; }

        public override IQuery Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (UserQueryParams) Convert.ChangeType(defaultParams, typeof(UserQueryParams));
            return this;
        }

        public override async Task<IEnumerable<T>> ToList<T>()
        {
            var builder = new UserQueryBuilder(this);

            // define all searchable input parameters 

            var users = await _store.SelectAsync<T>(
                PageIndex,
                PageSize,
                builder.BuildSqlStartId(),
                builder.BuildSqlPopulate(),
                builder.BuildSqlCount(),
                "@id int, @UserName nvarchar(255), @Email nvarchar(255)",
                Params.Id.Value,
                Params.UserName.Value,
                Params.Email.Value
            );


            return users;




        }
    }

    public class UserQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly UserQuery _query;

        public UserQueryBuilder(UserQuery query)
        {
            _query = query;
        }

        #endregion

        #region "Implementation"

        public string BuildSqlStartId()
        {
            var where = BuildWhere(false);
            var sb = new StringBuilder();
            sb.Append("SELECT @start_id_out = Id FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }

        public string BuildSqlPopulate()
        {
            var where = BuildWhere();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var where = BuildWhere();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM Plato_Users");
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE (").Append(where).Append(")");

            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        private string BuildWhere(bool includeStartId = true)
        {
            var sb = new StringBuilder();
            if (includeStartId)
                sb.Append("Id >= @start_id_in");

            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
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

        #endregion
    }
}