using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{

    public interface ISchemaBuilderBase
    {

        SchemaBuilderOptions Options { get; set; }

        ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure);

        ICollection<string> Statements { get; }

        ISchemaBuilderBase AddStatement(string statement);

        Task<string> Build();

        string ToString();

    }
    
    public interface ITableBuilder : ISchemaBuilderBase
    {
        ITableBuilder CreateTable(SchemaTable table);

        ITableBuilder AlterTableColumns(SchemaTable table);

        ITableBuilder DropTableColumns(SchemaTable table);

        ITableBuilder DropTable(SchemaTable table);

    }

    public interface IProcedureBuilder : ISchemaBuilderBase
    {

        IProcedureBuilder CreateProcedure(SchemaProcedure procedure);

        IProcedureBuilder DropProcedure(SchemaProcedure procedure);

        IProcedureBuilder AlterProcedure(SchemaProcedure procedure);

        IProcedureBuilder CreateDefaultProcedures(SchemaTable table);

        IProcedureBuilder DropDefaultProcedures(SchemaTable table);

    }
    
    public interface ISchemaBuilderFacade : IDisposable
    {

        ICollection<string> Statements { get; }

        SchemaBuilderOptions Options { get; }

        ISchemaBuilderFacade Configure(Action<SchemaBuilderOptions> configure);
        
        ITableBuilder TableBuilder { get; }

        IProcedureBuilder ProcedureBuilder { get; }
        
    }


    // --------------------- 

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
