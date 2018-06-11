using System;
using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data.Migrations
{
    public class DataMigrationManager : IDataMigrationManager
    {

        #region "Private Variables"
              
        private readonly IDbContext _dbContext;

        private List<Exception> _errors;
        
        #endregion

        #region "constructor"


        public DataMigrationManager(
            IDbContext dbContext)
        {
            _dbContext = dbContext;                  
        }

        #endregion

        #region "Implementation"
        
        public DataMigrationResult ApplyMigrations(DataMigrationRecord dataMigrationRecord)
        {
            var result = new DataMigrationResult();
            foreach (var migration in dataMigrationRecord.Migrations)
            {
                var commit = CommitMigration(migration);
                if (commit > 0)
                    result.SuccessfulMigrations.Add(migration);
                else
                    result.FailedMigrations.Add(migration);
            }
            result.Errors = _errors;
            return result;
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
                        migrationId = context.ExecuteScalar<int>(
                            System.Data.CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        if (_errors == null)
                            _errors = new List<Exception>();;
                        _errors.Add(ex);
                    }
                }
            }
            return migrationId;

        }

        #endregion

    }

}
