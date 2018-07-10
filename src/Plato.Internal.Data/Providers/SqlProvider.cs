using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data.Providers
{
    public class SqlProvider : IDataProvider, IDisposable
    {

        #region "Private Variables"

        private readonly string _connectionString;
        private SqlConnection _dbConnection;
    
        #endregion

        #region "Constructors"

        protected SqlProvider()
        {
        }

        public SqlProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region "Properties"
        
      
        public int CommandTimeout { get; set; }

        public IDbConnection Connection => _dbConnection;

        #endregion

        #region "Open / Close"


        public async Task OpenAsync()
        {

            if (String.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("The connection string has not been initialized.");
            }

            _dbConnection = new SqlConnection {ConnectionString = _connectionString};
            await _dbConnection.OpenAsync();

        }

        public void Close()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }

        #endregion

        #region "Implementation"
        
        public async Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args)
        {

            SqlDataReader reader;
            try
            {
                await OpenAsync();           
                using (var command = CreateCommand(_dbConnection, sql, args))
                {
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    OnExecutedCommand(command);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }

            return reader;

        }
     
        public async Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
        {

            object output = null;
            try
            {
                await OpenAsync();
                using (var cmd = CreateCommand(_dbConnection, sql, args))
                {
                    output = await cmd.ExecuteScalarAsync();
                    OnExecutedCommand(cmd);
                }
            }
            catch (Exception x)
            {
                HandleException(x);
                throw;
            }
            
            return (T)Convert.ChangeType(output, typeof(T));
            
        }

        public async Task<T> ExecuteAsync<T>(string sql, params object[] args)
        {
            try
            {
                await OpenAsync();
                using (var cmd = CreateCommand(_dbConnection, sql, args))
                {
                    var retv = await cmd.ExecuteNonQueryAsync();
                    OnExecutedCommand(cmd);
                    return (T)Convert.ChangeType(retv, typeof(T));
                }
            }
            catch (Exception x)
            {
                HandleException(x);
                throw;
            }

        }
        
        public void Dispose()
        {
            Close();
        }

        #endregion

        #region "Private Methods"

        SqlCommand CreateCommand(
            SqlConnection connection,
            string sql, 
            params object[] args)
        {

            // Create the command and add parameters
            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
          
            foreach (var item in args)
            {
                AddParam(cmd, item);                
            }

            if (!string.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }
        

        void DoPreExecute(IDbCommand cmd)
        {
            if (CommandTimeout != 0)
            {
                cmd.CommandTimeout = CommandTimeout;
            }
        }

        void AddParam(IDbCommand cmd, object item)
        {
            
            var p = cmd.CreateParameter();
            p.ParameterName = $"@{cmd.Parameters.Count}";
            
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                var t = item.GetType();
                if (t == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(byte[]))
                {
                    p.Value = item;
                    p.DbType = DbType.Binary;            
                }
                else if (t == typeof(string))
                {
                    p.Size = Math.Max(((string) item).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = item;
                }
                else if (t == typeof(bool))
                {
                    p.Value = ((bool)item) ? 1 : 0;
                    p.DbType = DbType.Boolean;
                }
                else if (t == typeof(int))
                {
                    p.Value = ((int)item);
                }
                else if (t == typeof(DateTime?))
                {
                    p.Value = ((DateTime) item);
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);

        }
        
        #endregion

        #region "Virtual Methods"

        // mainly used to hook in and override behaviour

        public virtual void OnExecutedCommand(IDbCommand cmd) { }
                
        public event DbEventHandlers.DbExceptionEventHandler OnException;  

        public virtual void HandleException(Exception x)
        {
            OnException?.Invoke(this, new DbExceptionEventArgs(x));
        }
     
        #endregion

    }
}
