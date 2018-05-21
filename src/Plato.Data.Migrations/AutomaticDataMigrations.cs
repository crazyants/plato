using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private readonly IDataMigrationBuilder _migrationBuilder;

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder)
        {
            _migrationBuilder = migrationBuilder;
        }

        #endregion

        #region "Private Methods"

        private void InitialMigration()
        {
            _migrationBuilder.BuildMigrations(
                new List<string>()
                {
                    "1.0.0"
                });
        }

        #endregion




    }
}
