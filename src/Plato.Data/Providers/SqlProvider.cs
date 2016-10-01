using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data
{
    public class SqlProvider : IDataProvider
    {

        #region "Private Variables"

        private string _connectionString;
        private IDbConnection _dbConnection;
        private DbTransaction _transaction;
        private int _sharedConnectionDepth;
        private int _oneTimeCommandTimeout;
        private string _lastSql;
        object[] _lastArgs;

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

        public bool KeepConnectionAlive { get; set; }

        public int OneTimeCommandTimeout { get; set; }

        public int CommandTimeout { get; set; }

        public DbTransaction Transaction
        {
            get
            {       
                return _transaction;
            }
        }

        #endregion

        #region "Open / Close"

        public void Open()
        {
            if (_sharedConnectionDepth == 0)
            {
                _dbConnection.ConnectionString = _connectionString;
                _dbConnection.Open();
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

        #region "Public Methods"

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
                OnException(exception);
                reader = null;
            }
        
            return reader;

        }

        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                Open();
                try
                {
                    using (var cmd = CreateCommand(_dbConnection, sql, args))
                    {
                        object val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);
                        return (T)Convert.ChangeType(val, typeof(T));
                    }
                }
                finally
                {
                    Close();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
                

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
                OnException(x);
                throw;
            }
        }

        #endregion

        #region "Private Methods"

        IDbCommand CreateCommand(
            IDbConnection connection,
            string sql, params object[] args)
        {
                      
            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            foreach (var item in args)
            {
                AddParam(cmd, item);
            }

            if (!String.IsNullOrEmpty(sql))
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

            string parameterPrefix = "@";

            // Support passed in parameters
            var idbParam = item as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
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
                else if (t == typeof(string))
                {
                    p.Size = Math.Max((item as string).Length + 1, 4000);		// Help query plan caching by using common size
                    p.Value = item;
                }             
                else if (t == typeof(bool))
                {
                    p.Value = ((bool)item) ? 1 : 0;
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

        public virtual void OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());       
        }

        public virtual void OnConnectionClosing(IDbConnection conn) { }

        public virtual void OnExecutingCommand(IDbCommand cmd) { }

        #endregion
        
    }
}
