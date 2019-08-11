using System;
using System.Text;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{

    // This implementation is simple and only intended to provides a
    // very thin wrapper around create, alter and delete for non-unique clustered indexes.
    // PRIMARY KEY or unique constraints should be handled separately

    // CREATE INDEX
    //https://docs.microsoft.com/en-us/sql/t-sql/statements/create-index-transact-sql?view=sql-server-2017

    // ALTER INDEX
    // https://docs.microsoft.com/en-us/sql/t-sql/statements/alter-index-transact-sql?view=sql-server-2017

    // DROP INDEX
    // https://docs.microsoft.com/en-us/sql/t-sql/statements/drop-index-transact-sql?view=sql-server-2017

    public class IndexBuilder : SchemaBuilderBase, IIndexBuilder
    {
        public IndexBuilder(
            IDbContext dbContext,
            IPluralize pluralize) : base(dbContext, pluralize)
        {
        }

        public IIndexBuilder CreateIndex(SchemaIndex index)
        {

            if (Options.DropIndexesBeforeCreate)
            {
                DropIndex(index);
            }

            AddStatement(CreateIndexInternal(index));
            return this;

        }

        public IIndexBuilder AlterIndex(SchemaIndex index)
        {
            AddStatement(AlterIndexInternal(index));
            return this;
        }

        public IIndexBuilder DropIndex(SchemaIndex index)
        {
            AddStatement(DropIndexInternal(index));
            return this;
        }

        // -------------------------------
        
        private string CreateIndexInternal(SchemaIndex index)
        {

            // CREATE INDEX IX_tableName_columnName ON tableName
            //    ( columnName(s) )
            // WITH(FILLFACTOR = 30)
            // GO

            var sb = new StringBuilder();

            var indexName = index.GenerateName();

            sb.Append("CREATE INDEX ")
                .Append(GetIndexName(indexName))
                .Append(" ON ")
                .Append(GetIndexName(index.TableName))
                .Append(" (")
                .Append(index.Columns.ToDelimitedString(','))
                .Append(")");

            if (index.FillFactor > 0)
            {
                sb.Append(" WITH (FILLFACTOR = ")
                    .Append(index.FillFactor)
                    .Append(")");
            }

            sb.Append(";");

            return sb.ToString();

        }

        private string AlterIndexInternal(SchemaIndex index)
        {

            // Defragment by physically removing rows that have been logically deleted from the table, and merging rowgroups.  
    
            var indexName = index.GenerateName();

            var sb = new StringBuilder();
            sb.Append("ALTER INDEX ")
                .Append(GetIndexName(indexName))
                .Append(" ON ")
                .Append(index.TableName)
                .Append(" REORGANIZE");

            return sb.ToString();

        }
        
        private string DropIndexInternal(SchemaIndex index)
        {

            // DROP INDEX
            //  IX_tableName_firstColumn ON tableName.columnName,  
            //  IX_tableName_firstColumn ON tableName.columnName;
            // GO

            var sb = new StringBuilder();

            var indexName = index.GenerateName();
            
            foreach (var column in index.Columns)
            {
                sb.Append("DROP INDEX ")
                    .Append(GetIndexName(indexName))
                    .Append(" ON ")
                    .Append(index.TableName)
                    .Append(".")
                    .Append(column)
                    .Append(NewLine);
            }
    
            return sb.ToString();

        }

    }
    
}
