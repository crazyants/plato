using System;
using System.Data;

namespace Plato.Internal.Data.Abstractions.Extensions
{
    public static class DbDataParameterExtensions
    {

        public static string DbTypeNormalized(this IDbDataParameter dbDataParameter)
        {
            var dbTypeNormalized = dbDataParameter.DbType.ToDbTypeNormalized(dbDataParameter.Size.ToString());
            if (String.IsNullOrEmpty(dbTypeNormalized))
            {
                throw new Exception($"Type not returned for column '{dbDataParameter.ParameterName}' whilst building shema");
            }
            return dbTypeNormalized;
        }
    }
}
