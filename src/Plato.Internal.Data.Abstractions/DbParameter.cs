using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Data.Abstractions.Extensions;

namespace Plato.Internal.Data.Abstractions
{

    public class DbParams : List<DbParam>
    {

    }

    public class DbParam
    {

        public string ParameterName { get; private set; }

        public object Value { get; private set; }
        
        public DbType DbType { get; set; }

        public int Size { get; set; }

        public ParameterDirection Direction { get; private set; }

        public DbParam(string parameterName, object value) : this(parameterName, Math.Max(((string)value).Length + 1, 4000), value, ParameterDirection.Input)
        {
        }
        public DbParam(string parameterName, int size, object value) : this(parameterName, size, value, ParameterDirection.Input)
        {
        }

        public DbParam(string parameterName, int size, object value, ParameterDirection direction)
        {

            ParameterName = parameterName;
            Direction = direction;
            
            // Automatically set the DbType based on our value type

            if (value == null)
            {
                Value = DBNull.Value;
            }
            else
            {

                var valueType = value.GetType();
                if (valueType == typeof(Guid))
                {
                    Value = value.ToString();
                    DbType = DbType.String;
                    Size = 40;
                }
                else if (valueType == typeof(byte[]))
                {
                    Value = value;
                    DbType = DbType.Binary;
                }
                else if (valueType == typeof(string))
                {
                    Size = size; 
                    Value = value;
                }
                else if (valueType == typeof(bool))
                {
                    Value = ((bool)value) ? 1 : 0;
                    DbType = DbType.Boolean;
                }
                else if (valueType == typeof(int))
                {
                    Value = ((int)value);
                }
                else if (valueType == typeof(DateTime?))
                {
                    Value = ((DateTime)value);
                }
               
                else
                {
                    Value = value;
                }
            }
            
        }
        
        public string DbTypeNormalized()
        {
            var dbTypeNormalized = DbType.ToDbTypeNormalized(Size.ToString());
            if (String.IsNullOrEmpty(dbTypeNormalized))
            {
                throw new Exception($"Type not returned for parameter '{ParameterName}' whilst building schema");
            }
            return dbTypeNormalized;
        }

        public IDbDataParameter CreateParameter(IDbCommand cmd)
        {

            var p = cmd.CreateParameter();
            p.ParameterName = $"@{this.ParameterName}";
            p.Size = this.Size;
            p.Value = this.Value;
            p.Direction = this.Direction;
            return p;

        }

    }
    
}
