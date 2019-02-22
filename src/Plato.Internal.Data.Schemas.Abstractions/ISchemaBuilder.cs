using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{

    public interface ISchemaBuilder : IDisposable
    {
        
        ISchemaBuilder Configure(Action<SchemaBuilderOptions> configure);

        ICollection<string> Statements { get; }

        Task<string> Build();

        string ToString();

        // ---------

        ISchemaBuilder CreateTable(SchemaTable table);

        ISchemaBuilder AlterTableColumns(SchemaTable table);

        ISchemaBuilder DropTableColumns(SchemaTable table);

        ISchemaBuilder DropTable(SchemaTable table);

        ISchemaBuilder DropProcedure(SchemaProcedure procedure);

        ISchemaBuilder AddStatement(string statement);

        ISchemaBuilder CreateProcedure(SchemaProcedure procedure);

        ISchemaBuilder AlterProcedure(SchemaProcedure procedure);

        ISchemaBuilder CreateDefaultProcedures(SchemaTable table);

        ISchemaBuilder DropDefaultProcedures(SchemaTable table);
        

    }

}
