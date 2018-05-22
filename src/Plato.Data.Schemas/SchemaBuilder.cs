using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Plato.Data.Abstractions;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Schemas
{
    

    public class SchemaBuilder : ISchemaBuilder
    {
        private readonly string _tablePrefix;
        private readonly List<string> _statements;

        public SchemaBuilder()
        {

        }

        public SchemaBuilder(
            IDbContext dbContext
        )
        {
            _tablePrefix = dbContext.Configuration.TablePrefix;
            _statements = new List<string>();
        }

        public ISchemaBuilder TableExists(string tableName)
        {

            return this;
        }


        public ISchemaBuilder CreateTable(
            string tableName,
            List<SchemaColumn> columns)
        {

            tableName = QualifiedTableName(tableName);

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ")
                .Append(tableName);
            
            if (columns.Any())
            {
                sb.Append("(");
                sb.Append(System.Environment.NewLine);

                var primaryKey = string.Empty;
                foreach (var column in columns)
                {
                    if (column.PrimaryKey)
                        primaryKey = column.Name;
                    sb.Append(CreateTableColumns(column));
                    sb.Append(System.Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(primaryKey))
                {
                    sb.Append("CONSTRAINT PK_")
                        .Append(tableName)
                        .Append("_")
                        .Append(primaryKey)
                        .Append(" PRIMARY KEY CLUSTERED ( ")
                        .Append(primaryKey)
                        .Append(" )");
                }
                
                sb.Append(")");
                sb.Append(System.Environment.NewLine);

            }

            sb.Append(System.Environment.NewLine)
                .Append("GO")
                .Append(System.Environment.NewLine);

            _statements.Add(sb.ToString());

            return this;
        }

        public SchemaBuilder DropTable(string tableName)
        {

            return this;
        }

        public SchemaBuilder CreateColumn(
            string tableName,
            SchemaColumn column)
        {
            return this;
        }

        public SchemaBuilder DropColumn(
            string tableName,
            string columnName)
        {
            return this;
        }

        public SchemaBuilder Execute()
        {

            foreach (var statement in _statements)
            {

            }

            return this;
        }

        private string QualifiedTableName(string tableName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + "_" + tableName
                : tableName;
        }

        private string CreateTableColumns(
            SchemaColumn column)
        {

            var sb = new StringBuilder();
            sb.Append(column.Name);
            
            if (!string.IsNullOrEmpty(column.DefaultValueNormalizsed) && !column.Nullable)
            {
                sb.Append(" DEFAULT (")
                    .Append(column.DefaultValueNormalizsed)
                    .Append(")");
            }

            sb.Append(" ")
                .Append(column.DbTypeNormalized)
                .Append(column.Nullable ? " NULL" : " NOT NULL");
            
            return sb.ToString();


        }


    }
}
