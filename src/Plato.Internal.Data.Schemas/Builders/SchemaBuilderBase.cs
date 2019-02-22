using System;
using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{
    public class SchemaBuilderBase : ISchemaBuilderBase
    {

        public string NewLine => Environment.NewLine;

        private readonly string _tablePrefix;
        private readonly IPluralize _pluralize;

        public string TablePrefix => _tablePrefix;

        public ICollection<string> Statements { get; }

        public SchemaBuilderOptions Options { get; set; }
        public ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure)
        {
            Options = new SchemaBuilderOptions();
            configure(Options);
            return this;
        }

        public SchemaBuilderBase(
            IDbContext dbContext,
            IPluralize pluralize)
        {
            _pluralize = pluralize;
            _tablePrefix = dbContext.Configuration.TablePrefix;
            Statements = new List<string>();
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

        public ISchemaBuilderBase AddStatement(string statement)
        {
            if (!string.IsNullOrEmpty(statement))
                Statements.Add(statement);
            return this;
        }

        public string ParseExplicitTSql(string input)
        {
            return input
                .Replace("{prefix}_", _tablePrefix)
                .Replace("  ", "")
                .Replace("      ", "");
        }

    }
    
}
