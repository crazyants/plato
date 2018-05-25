using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Plato.Data.Abstractions.Exceptions;
using Plato.Data.Abstractions.Providers;

namespace Plato.Data.Providers
{
    public class SqlProvider : IDataProvider, IDisposable
    {

        #region "Private Variables"

        private readonly string _connectionString;
        private readonly string _tablePrefix;
        private SqlConnection _dbConnection;
        private static int _sharedConnectionDepth;
        private int _oneTimeCommandTimeout;
        private string _lastSql;
        private object[] _lastArgs;

        #endregion

        #region "Constructors"

        protected SqlProvider()
        {
        }

        public SqlProvider(string connectionString)
            : this(connectionString, "", 120)
        {
                  
        }

        public SqlProvider(string connectionString, string tablePrefix)
      : this(connectionString, tablePrefix, 120)
        {
        }

        public SqlProvider(
            string connectionString, 
            string tablePrefix,
            int oneTimeCommandTimeout)
        {
            _connectionString = connectionString;
            _tablePrefix = tablePrefix;
            _oneTimeCommandTimeout = oneTimeCommandTimeout;
        }

        #endregion

        #region "Properties"

        public bool KeepConnectionAlive { get; set; }

        public int OneTimeCommandTimeout { get; set; }

        public int CommandTimeout { get; set; }

        public DbTransaction Transaction { get; }

        public IDbConnection Connection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region "Open / Close"

        public void Open()
        {
            if (_sharedConnectionDepth == 0)
            {
                _dbConnection = new SqlConnection
                {
                    ConnectionString = _connectionString
                };
                _dbConnection.Open();
                if (KeepConnectionAlive)
                    _sharedConnectionDepth++;
            }
            _sharedConnectionDepth++;
        }

        public async Task OpenAsync()
        {
            if (_sharedConnectionDepth == 0)
            {

                _dbConnection = new SqlConnection {ConnectionString = _connectionString};
                await _dbConnection.OpenAsync();
                if (KeepConnectionAlive)
                    _sharedConnectionDepth++;
            }
            _sharedConnectionDepth++;
        }
        
        public void Close()
        {
            if (_sharedConnectionDepth > 0)
            {
                _sharedConnectionDepth--;
                if (_sharedConnectionDepth == 0)
                {
                    OnConnectionClosing(_dbConnection);
                    _dbConnection.Dispose();
                    _dbConnection = null;
                }
            }
        }

        #endregion

        #region "Implementation"

        public IDataReader ExecuteReader(string sql, params object[] args)
        {

            System.Data.IDataReader reader;
            try
            {
                Open();
                using (IDbCommand command = CreateCommand(_dbConnection, sql, args))
                {
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    OnExecutedCommand(command);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                reader = null;
            }
        
            return reader;

        }

        public async Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args)
        {

            SqlDataReader reader;
            try
            {
                await OpenAsync();           
                using (SqlCommand command = CreateCommand(_dbConnection, sql, args))
                {
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    OnExecutedCommand(command);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                reader = null;
            }

            return reader;

        }
        
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            object output = null;
            try
            {
                Open();
                using (var cmd = CreateCommand(_dbConnection, sql, args))
                {
                    output = cmd.ExecuteScalar();
                    OnExecutedCommand(cmd);
                }

            }
            catch (Exception x)
            {
                HandleException(x);
                throw x;
            }
          
            return (T)Convert.ChangeType(output, typeof(T)); ;

        }

        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
        {

            object output = null;
            try
            {
                Open();
                using (var cmd = CreateCommand(_dbConnection, sql, args))
                {
                    output = cmd.ExecuteScalar();
                    OnExecutedCommand(cmd);
                }

            }
            catch (Exception x)
            {
                HandleException(x);
                throw x;
            }

        
            return Task.FromResult((T)Convert.ChangeType(output, typeof(T)));
            
        }
                
        public int Execute(string sql, params object[] args)
        {
            try
            {
                Open();
                try
                {
                    using (var cmd = CreateCommand(_dbConnection, sql, args))
                    {
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                }
                finally
                {
                    Close();
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
   
            if (OneTimeCommandTimeout != 0)
            {
                cmd.CommandTimeout = OneTimeCommandTimeout;
                _oneTimeCommandTimeout = 0;
            }
            else if (CommandTimeout != 0)
            {
                cmd.CommandTimeout = CommandTimeout;
            }
                       
            OnExecutingCommand(cmd);
                      
            _lastSql = cmd.CommandText;
            _lastArgs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();

        }

        void AddParam(IDbCommand cmd, object item)
        {

            var paramPrefix = "@";
                       
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", 
                paramPrefix, cmd.Parameters.Count);
            
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
                    p.Size = Math.Max((item as string).Length + 1, 4000);		// Help query plan caching by using common size
                    p.Value = item;
                }
                else if (t == typeof(bool))
                {
                    p.Value = ((bool)item) ? 1 : 0;
                }
                else if (t == typeof(int))
                {
                    p.Value = ((int)item);
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
            System.Diagnostics.Debug.WriteLine(x.ToString());
            OnException?.Invoke(this, new DbExceptionEventArgs(x));
        }
                  
        public virtual void OnConnectionClosing(IDbConnection conn) { }

        public virtual void OnExecutingCommand(IDbCommand cmd) { }
        
        #endregion

    }
}
