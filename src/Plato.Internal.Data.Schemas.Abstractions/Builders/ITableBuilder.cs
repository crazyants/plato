namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface ITableBuilder : ISchemaBuilderBase
    {
        ITableBuilder CreateTable(SchemaTable table);

        ITableBuilder AlterTableColumns(SchemaTable table);

        ITableBuilder DropTableColumns(SchemaTable table);

        ITableBuilder DropTable(SchemaTable table);

    }

}
