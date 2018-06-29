using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{

    public class SchemaBuilderOptions
    {
        public string Version = "1.0.0";

        public string ModuleName { get; set; }

        public bool DropTablesBeforeCreate { get; set; } = false;

        public bool DropProceduresBeforeCreate { get; set; } = false;

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

        ISchemaBuilder DropProcedure(SchemaProcedure procedure);

        ISchemaBuilder CreateStatement(string statement);

        ISchemaBuilder CreateProcedure(SchemaProcedure proecedure);

        ISchemaBuilder AlterProcedure(SchemaProcedure procedure);

        ISchemaBuilder CreateDefaultProcedures(SchemaTable table);

        ISchemaBuilder DropDefaultProcedures(SchemaTable table);

        ISchemaBuilder ApplySchema();

        Task<ISchemaBuilder> ApplySchemaAsync();

        string GetSingularizedTableName(SchemaTable table);

        string ToString();

    }
}
