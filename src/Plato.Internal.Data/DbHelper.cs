using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data
{

    /// <summary>
    /// User supplied input should never be passed into these methods.
    /// If you need to provide user supplied input please ensure you manually
    ///  implement parameterized stored procedures for any user input via IDbContext
    /// These methods are intended for internal use only where we can
    /// safely guarantee the validity of the supplied SQL to execute. 
    /// </summary>
    public class DbHelper : IDbHelper
    {

        private readonly IDbContext _dbContext;
        private readonly IOptions<DbContextOptions> _dbOptions;

        public DbHelper(
            IOptions<DbContextOptions> dbOptions,
            IDbContext dbContext)
        {
            _dbOptions = dbOptions;
            _dbContext = dbContext;
        }

        #region "Implementation"

        /// <summary>
        /// Unsafe SQL execution. Use only if you can guarantee the safety of the supplied TSQL code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql)
        {
            var output = default(T);
            using (var db = _dbContext)
            {
                var value = await db.ExecuteScalarAsync<T>(CommandType.Text, ReplaceTablePrefix(sql));
                if (value != null)
                {
                    output = value;
                }
            }

            return output;
        }

        /// <summary>
        /// Unsafe SQL execution. Use only if you can guarantee the safety of the supplied TSQL code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, IDictionary<string, string> replacements)
        {
            return await ExecuteScalarAsync<T>(PerformSqlReplacements(sql, replacements));
        }

        /// <summary>
        /// Unsafe SQL execution. Use only if you can guarantee the safety of the supplied TSQL code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="populate"></param>
        /// <returns></returns>
        public async Task<T> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, Task<T>> populate) where T : class
        {

            T output = null;
            using (var db = _dbContext)
            {
                output = await db.ExecuteReaderAsync<T>(CommandType.Text, ReplaceTablePrefix(sql), populate);
            }

            return output;

        }

        /// <summary>
        /// Unsafe SQL execution. Use only if you can guarantee the safety of the supplied TSQL code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="replacements"></param>
        /// <param name="populate"></param>
        /// <returns></returns>
        public async Task<T> ExecuteReaderAsync<T>(string sql, IDictionary<string, string> replacements, Func<DbDataReader, Task<T>> populate) where T : class
        {
            return await ExecuteReaderAsync<T>(PerformSqlReplacements(sql, replacements), populate);
        }

        #endregion

        #region "Private Methods"

        string ReplaceTablePrefix(string input)
        {
           return input.Replace("{prefix}_", _dbOptions.Value.TablePrefix);
        }
        
        string PerformSqlReplacements(string input, IDictionary<string, string> replacements)
        {
            if (replacements != null)
            {
                foreach (var replacement in replacements)
                {
                    input = input.Replace(replacement.Key, replacement.Value?.Replace("'", "''"));
                }
            }

            return input;
        }

        #endregion

    }

}
