using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data.Migrations
{
    public class DataMigrationManager : IDataMigrationManager
    {
        
        private readonly IDbContext _dbContext;
        private List<Exception> _errors;
    
        public DataMigrationManager(
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _errors = new List<Exception>();
        }

  
        #region "Implementation"
        
        public async Task<DataMigrationResult> ApplyMigrationsAsync(DataMigrationRecord dataMigrationRecord)
        {
            var result = new DataMigrationResult();
            foreach (var migration in dataMigrationRecord.Migrations)
            {
                var commit = await CommitMigrationAsync(migration);
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

        private async Task<int> CommitMigrationAsync(DataMigration migration)
        {

            var migrationId = 0;
            using (var context = _dbContext)
            {
                foreach (var statement in migration.Statements)
                {
                    try
                    {
                        migrationId = await context.ExecuteScalarAsync2<int>(
                            System.Data.CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        if (_errors == null)
                            _errors = new List<Exception>(); ;
                        _errors.Add(ex);
                    }
                }
            }
            return migrationId;

        }

        #endregion

    }

}
