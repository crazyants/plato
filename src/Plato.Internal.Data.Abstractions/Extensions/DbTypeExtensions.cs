using System;
using System.Data;

namespace Plato.Internal.Data.Abstractions.Extensions
{
    public static class DbTypeExtensions
    {

        public static string ToDbTypeNormalized(this DbType dbType, string stringLength = "") 
        {
            
            switch (dbType)
            {
                case DbType.Int16:
                    return "smallint";
                case DbType.Int64:
                    return "float";
                case DbType.Int32:
                    return "int";
                case DbType.Boolean:
                    return "bit";
                case DbType.AnsiString:
                    // No column length for strings would create invalid SQL
                    if (string.IsNullOrEmpty(stringLength))
                    {
                        throw new ArgumentNullException(nameof(stringLength));
                    }
                    return "varchar(" + stringLength + ")";
                case DbType.String:
                    // No column length for strings would create invalid SQL
                    if (string.IsNullOrEmpty(stringLength))
                    {
                        throw new ArgumentNullException(nameof(stringLength));
                    }
                    return "nvarchar(" + stringLength + ")";
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

            return "sql_variant";

        }

    }

}
