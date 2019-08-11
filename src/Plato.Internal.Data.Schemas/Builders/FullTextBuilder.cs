using System;
using System.Text;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{
    
    public class FullTextBuilder : SchemaBuilderBase, IFullTextBuilder
    {

        public FullTextBuilder(
            IDbContext dbContext,
            IPluralize pluralize) : base(dbContext, pluralize)
        {
        }

        public IFullTextBuilder CreateCatalog(string catalogName)
        {
            if (string.IsNullOrEmpty(catalogName))
            {
                throw new ArgumentNullException(nameof(catalogName));
            }

            if (Options != null)
            {
                if (Options.DropCatalogBeforeCreate)
                {
                    DropCatalog(catalogName);
                }
            }
       
            var sb = new StringBuilder();
            sb.Append("IF NOT EXISTS(SELECT name FROM sys.fulltext_catalogs WHERE name = '")
                .Append(catalogName)
                .Append("')")
                .Append("BEGIN")
                .Append(NewLine);
            sb.Append("CREATE FULLTEXT CATALOG ")
                .Append(catalogName)
                .Append(" AS DEFAULT;");
            sb.Append(NewLine)
                .Append("END");

            AddStatement(sb.ToString());
            return this;
        }

        public IFullTextBuilder DropCatalog(string catalogName)
        {

            if (string.IsNullOrEmpty(catalogName))
            {
                throw new ArgumentNullException(nameof(catalogName));
            }

            var sb = new StringBuilder();

            sb.Append("IF EXISTS(SELECT name FROM sys.fulltext_catalogs WHERE name = '")
                .Append(catalogName)
                .Append("')")
                .Append("BEGIN")
                .Append(NewLine);
            sb.Append("DROP FULLTEXT CATALOG ")
                .Append(catalogName)
                .Append(";");
            sb.Append(NewLine)
                .Append("END");

            AddStatement(sb.ToString());
            return this;

        }

        public IFullTextBuilder RebuildCatalog(string catalogName)
        {

            if (string.IsNullOrEmpty(catalogName))
            {
                throw new ArgumentNullException(nameof(catalogName));
            }

            var sb = new StringBuilder();

            // Ensure catalog exists
            sb.Append("IF EXISTS(SELECT name FROM sys.fulltext_catalogs WHERE name = '")
                .Append(catalogName)
                .Append("')")
                .Append("BEGIN")
                .Append(NewLine);

            // Rebuild
            sb.Append("ALTER FULLTEXT CATALOG ")
                .Append(catalogName)
                .Append(" REBUILD WITH ACCENT_SENSITIVITY=OFF;");

            sb.Append(NewLine)
                .Append("END");

            AddStatement(sb.ToString());
            return this;

        }
        
        public IFullTextBuilder CreateIndex(SchemaFullTextIndex index)
        {
            AddStatement(CreateOrAlterIndex(index));
            return this;
        }

        public IFullTextBuilder AlterIndex(SchemaFullTextIndex index)
        {
            AddStatement(CreateOrAlterIndex(index, true));
            return this;
        }

        public IFullTextBuilder DropIndex(string tableName, string columnName)
        {
            return DropIndexes(tableName, new string[]
            {
                columnName
            });
        }

        public IFullTextBuilder DropIndexes(string tableName, string[] columnNames)
        {

            // Disable index
            DisableIndex(tableName);

            var sb = new StringBuilder();
      
            // Check index exists
            sb.Append("IF EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'")
                .Append(PrependTablePrefix(tableName))
                .Append("'))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine);

            // Drop columns
            sb.Append("ALTER FULLTEXT INDEX ON ")
                .Append(PrependTablePrefix(tableName))
                .Append(" DROP (")
                .Append(columnNames.ToDelimitedString())
                .Append(");")
                .Append(NewLine);

            sb.Append("END");

            AddStatement(sb.ToString());
            return this;
        }

        public IFullTextBuilder DropIndexes(string tableName)
        {

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            // Disable index
            DisableIndex(tableName);

            var sb = new StringBuilder();

            // Check index exists
            sb.Append("IF EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'")
                .Append(PrependTablePrefix(tableName))
                .Append("'))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine);

            // Drop the index
            sb.Append("DROP FULLTEXT INDEX ON ")
                .Append(PrependTablePrefix(tableName))
                .Append(";")
                .Append(NewLine);

            sb.Append("END");

            AddStatement(sb.ToString());
            return this;

        }

        public IFullTextBuilder DisableIndex(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var sb = new StringBuilder();

            // Check index exists
            sb.Append("IF EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'")
                .Append(PrependTablePrefix(tableName))
                .Append("'))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine);

            // Drop the index
            sb.Append("ALTER FULLTEXT INDEX ON ")
                .Append(PrependTablePrefix(tableName))
                .Append(" DISABLE;")
                .Append(NewLine);

            sb.Append("END");

            AddStatement(sb.ToString());
            return this;

        }

        string CreateOrAlterIndex(SchemaFullTextIndex index, bool alter = false)
        {
            
            var sb = new StringBuilder();

            sb.Append(alter ? "ALTER" : "CREATE")
                .Append(" FULLTEXT INDEX ON ")
                .Append(PrependTablePrefix(index.TableName))
                .Append(" (")
                .Append(index.ColumnNames.ToDelimitedString())
                .Append(" LANGUAGE ")
                .Append(index.LanguageCode)
                .Append(") KEY INDEX ")
                .Append(index.PrimaryKeyName)
                .Append(" ON ")
                .Append(index.CatalogName)
                .Append(" WITH STOPLIST = SYSTEM;");

            return sb.ToString();

        }

    }

}
