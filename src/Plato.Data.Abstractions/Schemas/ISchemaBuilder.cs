using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Data.Abstractions.Schemas
{

    public class SchemaBuilderOptions
    {
        public string Version = "1.0.0";

        public string ModuleName { get; set; }

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

        ISchemaBuilder CreateProcedure(SchemaProcedure proecedure);

        ISchemaBuilder CreateDefaultProcedures(SchemaTable table);

        ISchemaBuilder ApplySchema();

        Task<ISchemaBuilder> ApplySchemaAsync();
        
        string ToString();

    }
}
