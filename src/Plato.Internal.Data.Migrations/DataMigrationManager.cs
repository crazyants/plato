using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Migrations.Abstractions;

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
                var success = await CommitMigrationAsync(migration);
                if (success)
                {
                    result.SuccessfulMigrations.Add(migration);
                }
                else
                {
                    result.FailedMigrations.Add(migration);
                }
            }
            result.Errors = _errors;
            return result;
        }

        #endregion

        #region "Private Methods"

        private async Task<bool> CommitMigrationAsync(DataMigration migration)
        {

            var success = true;
            using (var context = _dbContext)
            {
                foreach (var statement in migration.Statements)
                {
                    try
                    {
                       await context.ExecuteScalarAsync<int>(
                            System.Data.CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        if (_errors == null)
                            _errors = new List<Exception>(); ;
                        _errors.Add(ex);
                        success = false;
                    }
                }
            }
            return success;

        }

        #endregion

    }

}
