using System;
using System.Text;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Stores.Abstractions
{

    #region "IQueryBuilder"

    public interface IQueryBuilder
    {
        string BuildSqlStartId();

        string BuildSqlPopulate();

        string BuildSqlCount();
    }

    #endregion

    #region "WhereString"

    public class WhereString
    {
        private readonly StringBuilder _builder;
        private QueryOperator _operator = QueryOperator.And;

        public WhereString()
        {
            _builder = new StringBuilder();
        }

        public string Value { get; private set; }

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public WhereString Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereString And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public WhereString Equals(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} = @{0}");
            return this;
        }

        public WhereString StartsWith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} LIKE '%' + @{0}");
            return this;
        }

        public WhereString Endsith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} LIKE @{0} + '%'");
            return this;
        }

        public WhereString IsIn(string value, char delimiter = ',')
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder
                .Append("{0} IN ( SELECT * FROM plato_fn_ListToTable('")
                .Append(delimiter)
                .Append("', @{0})  )");
            return this;
        }

        public WhereString Like(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} LIKE '%' + @{0}");
            return this;
        }

        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
        }
    }

    #endregion

    #region "WhereInt"

    public class WhereInt
    {
        private readonly StringBuilder _builder;
        private QueryOperator _operator = QueryOperator.And;

        public WhereInt()
        {
            _builder = new StringBuilder();
        }

        public int Value { get; private set; }

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public WhereInt Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereInt And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public WhereInt Equals(int value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} = ").Append(value.ToString());
            return this;
        }

        public WhereInt LessThan(int value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} < ").Append(value.ToString()); ;
            return this;
        }

        public WhereInt LessThanOrEqual(int value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} <= ").Append(value.ToString()); ;
            return this;
        }

        public WhereInt GreaterThan(int value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} >= ").Append(value.ToString()); ;
            return this;
        }

        public WhereInt GreaterThanOrEqual(int value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} >= ").Append(value.ToString()); ; ;
            return this;
        }
        
        public WhereInt Between(int min, int max)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = max;
            _builder
                .Append("{0} >= ")
                .Append(min.ToString())
                .Append(" AND ")
                .Append("{0} <= ")
                .Append(max.ToString());
            return this;
        }

        public WhereInt IsIn(int[] value, char delimiter = ',')
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value[0];
            _builder
                .Append("{0} IN ( SELECT * FROM plato_fn_ListToTable('")
                .Append(delimiter)
                .Append("', '").
                Append(value.ToDelimitedString()).
                Append("' )");
            return this;
        }

        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
        }
    }

    #endregion

    #region "WhereBool"

    public class WhereBool
    {
        private readonly StringBuilder _builder;
        private QueryOperator _operator = QueryOperator.And;

        public WhereBool()
        {
            _builder = new StringBuilder();
            Value = false;
        }

        public WhereBool(bool value)
        {
            _builder = new StringBuilder();
            Value = value;
        }
        
        public bool Value { get; private set; }

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public WhereBool Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereBool And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public WhereBool True()
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = true;
            _builder.Append("{0} = 1");
            return this;
        }

        public WhereBool False()
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = false;
            _builder.Append("{0} = 0");
            return this;
        }

        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
        }
    }

    #endregion

    #region "WhereDate"

    public class WhereDate
    {
        private readonly StringBuilder _builder;
        private QueryOperator _operator = QueryOperator.And;

        public WhereDate()
        {
            _builder = new StringBuilder();
        }

        public DateTime? Value { get; private set; }

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public WhereDate Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereDate And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public WhereDate GreaterThan(DateTime value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = value;
            _builder.Append("{0} > @{0}");
            return this;
        }

        public WhereDate GreaterThanOrEqual(DateTime value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = value;
            _builder.Append("{0} >= @{0}");
            return this;
        }

        public WhereDate LessThan(DateTime value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = value;
            _builder.Append("{0} < @{0}");
            return this;
        }

        public WhereDate LessThanOrEqual(DateTime value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = value;
            _builder.Append("{0} <= @{0}");
            return this;
        }


        public WhereDate NotEqual(DateTime value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" ").Append(_operator.ToString().ToUpper()).Append(" ");
            Value = value;
            _builder.Append("{0} <> @{0}");
            return this;
        }

        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
        }

        string NormalizeDate(DateTime dateTime)
        {
            return dateTime.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
        }
    }
    
    #endregion
    
    #region "QueryOperator"

    public enum QueryOperator
    {
        And,
        Or
    }

    #endregion
    
}