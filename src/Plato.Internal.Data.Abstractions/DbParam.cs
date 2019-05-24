using System;
using System.Data;
using Plato.Internal.Data.Abstractions.Extensions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbParam : IDbDataParameter
    {

        public string ParameterName { get; set; }

        public object Value { get; set; }
        
        public DbType DbType { get; set; }

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public int Size { get; set; }

        public bool IsNullable { get; }
        
        public ParameterDirection Direction { get; set; }

        public string SourceColumn { get; set; }

        public DataRowVersion SourceVersion { get; set; }
        
        
        public DbParam(string name, DbType dbType, object value) 
            : this(name, dbType, 0, ParameterDirection.Input, false, value)
        {
        }

        public DbParam(string name, DbType dbType, ParameterDirection direction)
            : this(name, dbType, 0, direction, false, null)
        {
        }
        
        public DbParam(string name, DbType dbType, int size, object value) 
            : this(name, dbType, size, ParameterDirection.Input, false, value)
        {
        }

        public DbParam(
            string name,
            DbType dbType,
            int size,
            ParameterDirection direction,
            bool isNullable,
            object value)
        {
            ParameterName = name;
            Direction = direction;
            DbType = dbType;
            Size = size;
            IsNullable = isNullable;
            PrepareValue(value);
        }

        public void PrepareValue(object value)
        {

            if (value == null)
            {
                Value = DBNull.Value;
                return;
            }
            
            if (DbType == DbType.Guid)
            {
                Value = value.ToString();
                Size = 40;
            }
            else if (DbType == DbType.Binary)
            {
                Value = (byte[]) value;
            }
            else if (DbType == DbType.String)
            {
                Value = this.Size > 0
                    ? value.ToString().ToEmptyIfNull().TrimToSize(this.Size)
                    : value.ToString().ToEmptyIfNull();
            }
            else if (DbType == DbType.Boolean)
            {
                Value = ((bool) value) ? 1 : 0;
            }
            else if (DbType == DbType.Double)
            {
                Value = (double) value;
            }
            else if (DbType == DbType.Int32)
            {
                Value = ((int) value);
            }
            else if (DbType == DbType.Int64)
            {
                Value = ((Int64) value);
            }
            else if (DbType == DbType.DateTime)
            {
                Value = ((DateTime) value);
            }
            else if (DbType == DbType.DateTime2)
            {
                Value = ((DateTime?) value);
            }
            else if (DbType == DbType.DateTimeOffset)
            {
                Value = ((DateTimeOffset?) value);
            }
            else
            {
                Value = value;
            }
            
        }

        public string DbTypeNormalized()
        {
            var dbTypeNormalized = DbType.ToDbTypeNormalized(Size == 0 ? "max" : Size.ToString());
            if (String.IsNullOrEmpty(dbTypeNormalized))
            {
                throw new Exception($"Type not returned for parameter '{ParameterName}' whilst building schema");
            }
            return dbTypeNormalized;
        }

        public IDbDataParameter CreateParameter(IDbCommand cmd)
        {

            var p = cmd.CreateParameter();
            p.ParameterName = $"@{ParameterName}";
            p.Value = Value;
            p.Direction = Direction;
            p.DbType = DbType;
         
            if (DbType == DbType.String || DbType == DbType.AnsiString)
            {
                if (Size > 0)
                {
                    p.Size = Size;
                }
                else
                {
                    p.Size = Math.Max(((string) Value).Length + 1,
                        4000); // Help query plan caching by using common size;
                }
            }
            else
            {
                if (Size > 0)
                    p.Size = Size;
            }

            return p;

        }

    }
    
}
