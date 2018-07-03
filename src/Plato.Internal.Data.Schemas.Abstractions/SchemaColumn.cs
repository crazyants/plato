using System;
using System.Data;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaColumn
    {

        public bool PrimaryKey { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Returns a version of the column Name safe for inclusion within @paramter arguments. 
        /// </summary>
        public string NameNormalized => this.Name.Replace("[", "").Replace("]", "");

        public DbType DbType { get; set; }

        public string Length { get; set; }
        
        public string DefaultValue { get; set; }

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
                        return "GetDate()";
                    case DbType.DateTime2:
                        return "NULL";
                    case DbType.DateTime:
                        return "GetDate()";
                    case DbType.DateTimeOffset:
                        return "GetDate()";
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
                switch (this.DbType)
                {
                    case DbType.Int16:
                        return "short";
                    case DbType.Int64:
                        return "float";
                    case DbType.Int32:
                        return "int";
                    case DbType.Boolean:
                        return "bit";
                    case DbType.String:
                        return "nvarchar(" + this.Length + ")";
                    case DbType.Date:
                        return "datetime";
                    case DbType.DateTime:
                        return "datetime";
                    case DbType.DateTime2:
                        return "datetime2";
                    case DbType.Binary:
                        return "varbinary(max)";
                    case DbType.DateTimeOffset:
                        return "datetimeoffset";
                    case DbType.Decimal:
                        return "decimal";
                    case DbType.Double:
                        return "float";
                    case DbType.Guid:
                        return "uniqueidentifier";
                    case DbType.Xml:
                        return "xml";

                }

                throw new Exception($"Type not returned for column '{this.Name}' whilst building shema");
                
            }
        }

        public bool Nullable { get; set; }

    }


}
