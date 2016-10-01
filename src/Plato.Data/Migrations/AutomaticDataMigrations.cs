using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private IDataMigrationManager _migrationManager;

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        #endregion

        #region "Private Methods"

        private void InitialMigration()
        {

            DataMigration initialMigration = new DataMigration()
            {
                Version = 1,
                Upgrade = @"

                    CREATE TABLE 
                
                ",
                RollBack = @""
            };

            var migrations = new DataMigrationRecord();
            migrations.Migrations.Add(initialMigration);


            _migrationManager.ApplyMigrations(migrations);


        }

        #endregion




    }
}
