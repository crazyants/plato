using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{

    public class SchemaBuilderOptions
    {
        public string Version = "1.0.0";
    }
    
    public interface ISchemaBuilder : IDisposable
    {

        List<string> Statements { get; }

        List<Exception> Errors { get; }

        ISchemaBuilder Configure(Action<SchemaBuilderOptions> configure);

        ISchemaBuilder CreateTable(SchemaTable table);

        ISchemaBuilder AlterTableColumns(SchemaTable table);

        ISchemaBuilder DropTableColumns(SchemaTable table);

        ISchemaBuilder DropTable(SchemaTable table);

        ISchemaBuilder CreateStatement(string statement);

        ISchemaBuilder CreateStoredProcedure(SchemaStoredProcedure storedProecedure);

        ISchemaBuilder Apply();

        string ToString();

    }
}
