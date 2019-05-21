using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{
    
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
            throw new NotImplementedException();
        }

        public IIndexBuilder DropIndex(SchemaIndex index)
        {
            throw new NotImplementedException();
        }

        // -------------------------------


        private string CreateIndexInternal(SchemaIndex index)
        {

            // CREATE INDEX IX_tableName_columnName ON tableName
            //    ( columnName(s) )
            // WITH(FILLFACTOR = 30)
            
            var sb = new StringBuilder();

            var indexName = index.GenerateName();

            sb.Append("CREATE INDEX ")
                .Append(GetIndexName(indexName))
                .Append(NewLine)
                .Append("( ")
                .Append(index.Columns.ToDelimitedString(','))
                .Append(" )");

            if (index.FillFactor > 0)
            {
                sb.Append(" WITH (FILLFACTOR = ")
                    .Append(index.FillFactor)
                    .Append(")");
            }

            return sb.ToString();

        }

    }


}
