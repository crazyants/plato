using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Migrations
{
    public interface IDataMigrationBuilder
    {

        void BuildMigrations(List<string> versions);
    }
}
