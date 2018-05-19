using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public class DataMigrationManager : IDataMigrationManager
    {

        #region "Private Variables"
              
        private IDbContext _dbContext;

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

            var completedMigrations = new List<DataMigration>();
            foreach (var migration in dataMigrationRecord.Migrations)
            {
                var commit = CommitMigration(migration);
                if (commit > 0)
                    _successfulMigrations.Add(migration);
                else
                    _failedMigrations.Add(migration);
            }

            return completedMigrations;
        }

        #endregion

        #region "Private Methods"

        private int CommitMigration(DataMigration migration)
        {

            int migrationID = 0;

            using (var context = _dbContext)
            {

                try
                {

                    migrationID = context.ExecuteScalar<int>(
                        System.Data.CommandType.Text, migration.Upgrade);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            return migrationID;

        }

        #endregion

    }

}
