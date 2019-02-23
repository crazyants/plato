namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface IFullTextBuilder : ISchemaBuilderBase
    {

        IFullTextBuilder CreateCatalog(string catalogName);

        IFullTextBuilder DropCatalog(string catalogName);

        IFullTextBuilder CreateIndex(SchemaFullTextIndex index);

        IFullTextBuilder AlterIndex(SchemaFullTextIndex index);

        IFullTextBuilder DropIndex(string tableName, string columnName);

        IFullTextBuilder DropIndexes(string tableName, string[] columnNames);


        IFullTextBuilder DropIndexes(string tableName);
        
    }
    
}
