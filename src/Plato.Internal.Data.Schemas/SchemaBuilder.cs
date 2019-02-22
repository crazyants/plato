using System;
using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Data.Schemas.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas
{
    
    public class SchemaBuilder : ISchemaBuilder
    {

        public ICollection<string> Statements
        {
            get
            {

                var statements = new List<string>();
                foreach (var statement in TableBuilder.Statements)
                {
                    statements.Add(statement);
                }

                foreach (var statement in ProcedureBuilder.Statements)
                {
                    statements.Add(statement);
                }

                return statements;

            }
        }

        public SchemaBuilderOptions Options { get; private set; }

        public ITableBuilder TableBuilder { get; }

        public IProcedureBuilder ProcedureBuilder { get; }

        public IFullTextBuilder FullTextBuilder { get; }

        public SchemaBuilder(
            IDbContext dbContext,
            IPluralize pluralize) 
        {
            ProcedureBuilder = new ProcedureBuilder(dbContext, pluralize);
            TableBuilder = new TableBuilder(dbContext, pluralize);
            FullTextBuilder = new FullTextBuilder(dbContext, pluralize);
        }
        
        public ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure)
        {

            Options = new SchemaBuilderOptions();
            configure(Options);

            // Configure builders
            TableBuilder.Configure(configure);
            ProcedureBuilder.Configure(configure);
            FullTextBuilder.Configure(configure);

            return this;
        }
        
        public void Dispose()
        {
        }

    }
    
}
