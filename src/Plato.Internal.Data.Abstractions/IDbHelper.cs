using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDbHelper
    {

        Task<T> ExecuteScalarAsync<T>(string sql);

        Task<T> ExecuteScalarAsync<T>(string sql, IDictionary<string, string> replacements);

        Task<T> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, Task<T>> populate) where T : class;

        Task<T> ExecuteReaderAsync<T>(string sql, IDictionary<string, string> replacements,
            Func<DbDataReader, Task<T>> populate) where T : class;

    }

}
