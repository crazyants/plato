using System;
using System.Linq;
using System.Text;
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
            
            if (Options.DropCatalogBeforeCreate)
            {
                DropCatalog(catalogName);
            }

            var sb = new StringBuilder();
            sb.Append("IF NOT EXISTS(SELECT name FROM sys.fulltext_catalogs WHERE name = '")
                .Append(catalogName)
                .Append("'))")
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

        public IFullTextBuilder CreateIndex(SchemaFullTextIndex index)
        {
     
            var sb = new StringBuilder();

            sb.Append("CREATE FULLTEXT INDEX ON ")
                .Append(index.TableName)
                .Append("(");

            var i = 0;
            var len = index.ColumnNames.Count();
            foreach (var name in index.ColumnNames)
            {
                sb.Append(name);
                i++;
                if (i < len)
                {
                    sb.Append(",");
                }
            }
            
            sb.Append(" LANGUAGE ")
                .Append(index.LanguageCode)
                .Append(")")
                .Append("KEY INDEX ")
                .Append(index.PrimaryKeyName)
                .Append("ON ")
                .Append(index.CatalogName)
                .Append("WITH STOPLIST = SYSTEM");

            AddStatement(sb.ToString());
            return this;

        }

        public IFullTextBuilder DropIndex(string column)
        {
            // TODO
            throw new NotImplementedException();
        }

        public IFullTextBuilder DropIndexes(string tableName)
        {

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var sb = new StringBuilder();
            sb.Append("DROP FULLTEXT INDEX ON ")
                .Append(tableName)
                .Append(";");

            AddStatement(sb.ToString());
            return this;

        }

    }

}
