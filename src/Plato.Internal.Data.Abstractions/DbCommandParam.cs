using System.Data;

namespace Plato.Internal.Data.Abstractions
{
 
    public class DbDataParameter : IDbDataParameter
    {
   
        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsNullable { get; }

        public string ParameterName { get; set; }

        public string SourceColumn { get; set; }

        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public int Size { get; set; }
        
        public DbDataParameter(bool isNullable)
        {
            IsNullable = isNullable;
        }

        public DbDataParameter(
            DbType dbType,
            ParameterDirection direction)
        {
            this.DbType = dbType;
            this.Direction = direction;
        }
        
    }
    
}
