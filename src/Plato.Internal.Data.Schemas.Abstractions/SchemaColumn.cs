using System;
using System.Data;
using Plato.Internal.Data.Abstractions.Extensions;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaColumn
    {

        public bool PrimaryKey { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Returns a version of the column Name safe for inclusion within @parameter arguments. 
        /// </summary>
        public string NameNormalized => this.Name.Replace("[", "").Replace("]", "");

        public DbType DbType { get; set; }

        public string Length { get; set; }
        
        public string DefaultValue { get; set; }

        public Direction Direction { get; set; } = Direction.In;

        public string DefaultValueNormalizsed
        {
            get
            {
                if (!string.IsNullOrEmpty(DefaultValue))
                    return DefaultValue.ToUpper();
                switch (this.DbType)
                {
                    case DbType.Int16:
                        return "0";
                    case DbType.Int32:
                        return "0";
                    case DbType.Int64:
                        return "0";
                    case DbType.Boolean:
                        return "0";
                    case DbType.String:
                        return "''";
                    case DbType.Date:
                        return "GETUTCDATE()";
                    case DbType.DateTime2:
                        return "NULL";
                    case DbType.DateTime:
                        return "GETUTCDATE()";
                    case DbType.DateTimeOffset:
                        return "GETUTCDATE()";
                    case DbType.Binary:
                        return "NULL";
                    case DbType.Decimal:
                        return "0";
                    case DbType.Double:
                        return "0";
                    case DbType.Guid:
                        return "''";
                    case DbType.Xml:
                        return "''";
                }

                throw new Exception($"Default value not returned for column '{this.Name}' whilst attempting to find default value for {this.DbType.ToString()}");
                
            }

        }
        
        public string DbTypeNormalized
        {
            get
            {
                var dbTypeNormalized = this.DbType.ToDbTypeNormalized(this.Length);
                if (String.IsNullOrEmpty(dbTypeNormalized))
                {
                    throw new Exception($"Type not returned for column '{this.Name}' whilst building shema");
                }
                return dbTypeNormalized;
            }
        }

        public bool Nullable { get; set; }

    }

    public enum Direction {
        In,
        Out
    }

}
