using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Stores.Abstractions
{

    #region "IQueryBuilder"

    public interface IQueryBuilder
    {
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

        public WhereString(int parameterIndex) : this()
        {
            ParameterIndex = parameterIndex;
        }


        public string Value { get; private set; }

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public int ParameterIndex { get; }

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

        public WhereString Operand(QueryOperator queryOperator)
        {
            _operator = queryOperator;
            return this;
        }


        public WhereString Equals(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(this.Operator);
            Value = value;
            _builder.Append("{columnName} = @{paramName}");
            return this;
        }

        public WhereString StartsWith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(this.Operator);
            Value = value;
            _builder.Append("{columnName} LIKE @{paramName} + '%'");
            return this;
        }

        public WhereString EndsWith(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(this.Operator);
            Value = value;
            _builder.Append("{columnName} LIKE '%' + @{paramName}");
            return this;
        }

        public WhereString Like(string value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(this.Operator);
            Value = value;
            _builder.Append("{columnName} LIKE '%' + @{paramName} + '%'");
            return this;
        }
        
        public string ToSqlString(string columnName, string paramName)
        {
            return _builder
                .ToString()
                .Replace("{columnName}", columnName)
                .Replace("{paramName}", paramName);
        }


    }

    #endregion

    #region "WhereStringArray"

    // TODO: Do we need to implement support for this?
    public class WhereStringArray : List<WhereString>
    {

        private QueryOperator _operator = QueryOperator.And;

        public WhereStringArray Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereStringArray And()
        {
            _operator = QueryOperator.And;
            return this;
        }


        public WhereStringArray Equals(string value)
        {
            var whereString = new WhereString();
            this.Add(whereString.Equals(value).Operand(_operator));
            return this;
        }

        public WhereStringArray Like(string value)
        {
            var whereString = new WhereString();
            this.Add(whereString.Like(value).Operand(_operator));
            return this;
        }

        public WhereStringArray StartsWith(string value)
        {
            var whereString = new WhereString();
            this.Add(whereString.StartsWith(value).Operand(_operator));
            return this;
        }

        public WhereStringArray EndsWith(string value)
        {
            var whereString = new WhereString();
            this.Add(whereString.EndsWith(value).Operand(_operator));
            return this;
        }

        public string Values()
        {
            var i = 0;
            var sb = new StringBuilder();
            foreach (var whereString in this)
            {
                sb.Append(whereString.Value);
                if (i < this.Count)
                    sb.Append(",");
                i++;
            }

            return sb.ToString();

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

        public int Value { get; private set; } = -1; // use negative 1 as the default to allow us to query for zeros

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
        
        public WhereInt Equals(int value, Action<StringBuilder> builder)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            _builder.Append("{0} = ").Append(value.ToString());
            builder(_builder);
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

            if (value == null || value.Length == 0)
            {
                return this;
            }

            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value[0];
            _builder
                .Append("{0} IN (")
                .Append(value.ToDelimitedString(delimiter))
                .Append(")");

            return this;

        }

        public string ToSqlString(string columnName)
        {
            return _builder.ToString().Replace("{0}", columnName);
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

    #region "WhereEnum"

    public class WhereEnum<T> where T : struct
    {

        private readonly StringBuilder _builder;
        private QueryOperator _operator = QueryOperator.And;

        public string Operator => _operator == QueryOperator.And ? " AND " : " OR ";

        public T Value { get; private set; }
        
        public WhereEnum()
        {
            _builder = new StringBuilder();
        }
        
        public WhereEnum<T> Or()
        {
            _operator = QueryOperator.Or;
            return this;
        }

        public WhereEnum<T> And()
        {
            _operator = QueryOperator.And;
            return this;
        }

        public WhereEnum<T> Equals(T value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            if (Enum.TryParse<T>(value.ToString(), true, out var result))
            {
                _builder.Append("{0} = ").Append(Convert.ToInt32(result));
            }
            return this;
        }

        public WhereEnum<T> NotEqual(T value)
        {
            if (!string.IsNullOrEmpty(_builder.ToString()))
                _builder.Append(" OR ");
            Value = value;
            if (Enum.TryParse<T>(value.ToString(), true, out var result))
            {
                _builder.Append("{0} <> ").Append(Convert.ToInt32(result));
            }
            return this;
        }

        public string ToSqlString(string parameterName)
        {
            return _builder.ToString().Replace("{0}", parameterName);
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