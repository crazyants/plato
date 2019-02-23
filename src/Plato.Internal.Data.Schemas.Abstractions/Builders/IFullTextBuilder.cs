namespace Plato.Internal.Data.Schemas.Abstractions.Builders
{
    public interface IFullTextBuilder : ISchemaBuilderBase
    {

        IFullTextBuilder CreateCatalog(string catalogName);

        IFullTextBuilder DropCatalog(string catalogName);

        IFullTextBuilder CreateIndex(SchemaFullTextIndex index);

        IFullTextBuilder DropIndex(string column);

        IFullTextBuilder DropIndexes(string tableName);
        
    }
    
}
