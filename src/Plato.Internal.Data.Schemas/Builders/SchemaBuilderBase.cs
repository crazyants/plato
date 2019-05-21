using System;
using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{

    /// <summary>
    /// A common base class to assist various schema builders.
    /// </summary>
    public class SchemaBuilderBase : ISchemaBuilderBase
    {

        public string NewLine => Environment.NewLine;
        
        public ICollection<string> Statements { get; }

        public SchemaBuilderOptions Options { get; private set; }

        private readonly string _tablePrefix;

        private readonly IPluralize _pluralize;

        public SchemaBuilderBase(
            IDbContext dbContext,
            IPluralize pluralize)
        {
            _pluralize = pluralize;
            _tablePrefix = dbContext.Configuration.TablePrefix;
            Statements = new List<string>();
        }

        public ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure)
        {
            Options = new SchemaBuilderOptions();
            configure(Options);
            return this;
        }

        public string GetTableName(string tableName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + tableName
                : tableName;
        }

        public string GetSingularTableName(string tableName)
        {
            return _pluralize.Singular(tableName);
        }

        public string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + procedureName
                : procedureName;
        }
        public string GetIndexName(string indexName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + indexName
                : indexName;
        }


        public ISchemaBuilderBase AddStatement(string statement)
        {
            if (!string.IsNullOrEmpty(statement))
                Statements.Add(statement);
            return this;
        }

        public string ParseExplicitSql(string input)
        {
            return input
                .Replace("{prefix}_", _tablePrefix)
                .Replace("  ", "")
                .Replace("      ", "");
        }

        public void Dispose()
        {
            this.Statements.Clear();
        }

    }
    
}
