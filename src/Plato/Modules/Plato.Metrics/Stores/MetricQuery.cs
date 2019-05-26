using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Metrics.Models;

namespace Plato.Metrics.Stores
{

    #region "MetricQuery"

    public class MetricQuery : DefaultQuery<Metric>
    {

        private readonly IStore<Metric> _store;

        public MetricQuery(IStore<Metric> store)
        {
            _store = store;
        }

        public MetricQueryParams Params { get; set; }

        public override IQuery<Metric> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (MetricQueryParams)Convert.ChangeType(defaultParams, typeof(MetricQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Metric>> ToList()
        {

            var builder = new MetricQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var title = Params.Title.Value ?? string.Empty;
            var url = Params.Url.Value ?? string.Empty;
            var ipV4Address = Params.IpV4Address.Value ?? string.Empty;
            var ipV6Address = Params.IpV6Address.Value ?? string.Empty;
            var userAgent = Params.UserAgent.Value ?? string.Empty;
            
            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Title", DbType.String, title),
                new DbParam("Url", DbType.String, url),
                new DbParam("IpV4Address", DbType.String, ipV4Address),
                new DbParam("IpV6Address", DbType.String, ipV6Address),
                new DbParam("UserAgent", DbType.String, userAgent),
            });

        }


    }

    #endregion

    #region "MetricQueryParams"

    public class MetricQueryParams
    {


        private WhereInt _id;
        private WhereInt _featureId;
        private WhereDate _startDate;
        private WhereDate _endDate;
        private WhereString _title;
        private WhereString _url;
        private WhereString _ipV4Address;
        private WhereString _ipV6Address;
        private WhereString _userAgent;
        private WhereInt _createdUserId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt FeatureId
        {
            get => _featureId ?? (_featureId = new WhereInt());
            set => _featureId = value;
        }
        public WhereDate StartDate
        {
            get => _startDate ?? (_startDate = new WhereDate());
            set => _startDate = value;
        }

        public WhereDate EndDate
        {
            get => _endDate ?? (_endDate = new WhereDate());
            set => _endDate = value;
        }
        
        public WhereString Title
        {
            get => _title ?? (_title = new WhereString());
            set => _title = value;
        }

        public WhereString Url
        {
            get => _url ?? (_url = new WhereString());
            set => _url = value;
        }

        public WhereString IpV4Address
        {
            get => _ipV4Address ?? (_ipV4Address = new WhereString());
            set => _ipV4Address = value;
        }

        public WhereString IpV6Address
        {
            get => _ipV6Address ?? (_ipV6Address = new WhereString());
            set => _ipV6Address = value;
        }

        public WhereString UserAgent
        {
            get => _userAgent ?? (_userAgent = new WhereString());
            set => _userAgent = value;
        }

        public WhereInt CreatedUserId
        {
            get => _createdUserId ?? (_createdUserId = new WhereInt());
            set => _createdUserId = value;
        }


    }

    #endregion

    #region "MetricQueryBuilder"

    public class MetricQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _metricTableName;
        private readonly string _usersTableName;
        private readonly MetricQuery _query;

        public MetricQueryBuilder(MetricQuery query)
        {
            _query = query;
            _metricTableName = GetTableNameWithPrefix("Metrics");
            _usersTableName = GetTableNameWithPrefix("Users");

        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {

            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();

            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());

            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(m.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("m.*, ")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.Alias,")
                .Append("u.PhotoUrl,")
                .Append("u.PhotoColor");
            return sb.ToString();

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_metricTableName)
                .Append(" m ");

            // join user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON m.CreatedUserId = u.Id ");
            return sb.ToString();

        }

        #endregion

        #region "Private Methods"

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("m.Id"));
            }

            // FeatureId
            if (_query.Params.FeatureId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FeatureId.Operator);
                sb.Append(_query.Params.FeatureId.ToSqlString("m.FeatureId"));
            }

            // StartDate
            if (_query.Params.StartDate.Value != null)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.StartDate.Operator);
                sb.Append(_query.Params.StartDate.ToSqlString("m.CreatedDate"));
            }

            // EndDate
            if (_query.Params.EndDate.Value != null)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EndDate.Operator);
                sb.Append(_query.Params.EndDate.ToSqlString("m.CreatedDate"));
            }

            // Title
            if (!String.IsNullOrEmpty(_query.Params.Title.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Title.Operator);
                sb.Append(_query.Params.Title.ToSqlString("m.Title", "Title"));
            }

            // Url
            if (!String.IsNullOrEmpty(_query.Params.Url.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Url.Operator);
                sb.Append(_query.Params.Url.ToSqlString("m.Url", "Url"));
            }
            
            // IpV4Address
            if (!String.IsNullOrEmpty(_query.Params.IpV4Address.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IpV4Address.Operator);
                sb.Append(_query.Params.IpV4Address.ToSqlString("m.IpV4Address", "IpV4Address"));
            }

            // IpV6Address
            if (!String.IsNullOrEmpty(_query.Params.IpV6Address.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IpV6Address.Operator);
                sb.Append(_query.Params.IpV6Address.ToSqlString("m.IpV6Address", "IpV6Address"));
            }

            // UserAgent
            if (!String.IsNullOrEmpty(_query.Params.UserAgent.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserAgent.Operator);
                sb.Append(_query.Params.UserAgent.ToSqlString("m.UserAgent", "UserAgent"));
            }

            // CreatedUserId
            if (_query.Params.CreatedUserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CreatedUserId.Operator);
                sb.Append(_query.Params.CreatedUserId.ToSqlString("er.CreatedUserId"));
            }


            return sb.ToString();

        }

        string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "er." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in GetSafeSortColumns())
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        IDictionary<string, OrderBy> GetSafeSortColumns()
        {
            var output = new Dictionary<string, OrderBy>();
            foreach (var sortColumn in _query.SortColumns)
            {
                var columnName = GetSortColumn(sortColumn.Key);
                if (String.IsNullOrEmpty(columnName))
                {
                    throw new Exception($"No sort column could be found for the supplied key of '{sortColumn.Key}'");
                }
                output.Add(columnName, sortColumn.Value);

            }

            return output;
        }

        string GetSortColumn(string columnName)
        {

            if (String.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            switch (columnName.ToLowerInvariant())
            {
                case "id":
                    return "m.Id";
                case "featureid":
                    return "m.FeatureId";
                case "title":
                    return "m.Title";
                case "url":
                    return "m.Url";
                case "ipaddress":
                    return "m.IpV4Address";
                case "ipv4address":
                    return "m.IpV4Address";
                case "ipv6address":
                    return "m.IpV4Address";
                case "useragent":
                    return "m.UserAgent";
                case "createduserid":
                    return "m.CreatedUserId";
                case "created":
                    return "m.CreatedDate";
                case "createddate":
                    return "m.CreatedDate";
            }

            return string.Empty;

        }

        #endregion

    }

    #endregion

}
