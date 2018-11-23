using System.Data;

namespace Plato.Internal.Data.Abstractions.Extensions
{
    public static class DbTypeExtensions
    {

        public static string ToSqlDbTypeNormalized(this DbType dbType, string stringLength = "") 
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
                case DbType.String:
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

            return string.Empty;
        }
    }
}
