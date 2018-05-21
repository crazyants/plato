using System;
using System.Collections.Generic;
using Plato.Data.Abstractions;

namespace Plato.Data.Migrations
{
    public class DataMigrationManager : IDataMigrationManager
    {

        #region "Private Variables"
              
        private readonly IDbContext _dbContext;
        private List<DataMigration> _successfulMigrations;
        private List<DataMigration> _failedMigrations;

        #endregion

        #region "constructor"

        public DataMigrationManager(
            IDbContext dbContext)
        {
            _dbContext = dbContext;                  
        }

        #endregion

        #region "Implementation"
        
        public IEnumerable<DataMigration> ApplyMigrations(DataMigrationRecord dataMigrationRecord)
        {

            if (_successfulMigrations == null)
                _successfulMigrations = new List<DataMigration>();
            if (_failedMigrations == null)
                _failedMigrations = new List<DataMigration>();

            var completedMigrations = new List<DataMigration>();
            foreach (var migration in dataMigrationRecord.Migrations)
            {
                var commit = CommitMigration(migration);
                if (commit > 0)
                    _successfulMigrations.Add(migration);
                else
                    _failedMigrations.Add(migration);
            }

            if (_successfulMigrations.Count > 0)
                completedMigrations.AddRange(_successfulMigrations);
            
            return completedMigrations;

        }

        #endregion

        #region "Private Methods"

        private int CommitMigration(DataMigration migration)
        {

            var migrationId = 0;
            using (var context = _dbContext)
            {
                foreach (var statement in migration.Statements)
                {
                    try
                    {
                        //migrationId = context.ExecuteScalar<int>(
                        //    System.Data.CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
              
            }

            return migrationId;

        }

        #endregion

    }

}
