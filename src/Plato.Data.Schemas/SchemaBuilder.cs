using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Plato.Data.Abstractions;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Schemas
{

    public class SchemaBuilder : ISchemaBuilder, IDisposable
    {
        private readonly string _tablePrefix;

        private List<string> _statements;
        private List<Exception> _errors;

        public List<Exception> Errors => _errors;

        public List<string> Statements
        {
            get => _statements ?? (_statements = new List<string>());
            set => _statements = value;
        }

        private readonly IDbContext _dbContext;
        
        public SchemaBuilder(
            IDbContext dbContext
        )
        {
            _tablePrefix = dbContext.Configuration.TablePrefix;
            _dbContext = dbContext;

            _statements = new List<string>();
            _errors = new List<Exception>();

        }

        #region "Implementation"

        private SchemaBuilderOptions _options;

        public ISchemaBuilder Configure(Action<SchemaBuilderOptions> configure)
        {
            _options = new SchemaBuilderOptions();
            configure(_options);
            return this;
        }
        
        public ISchemaBuilder CreateTable(SchemaTable table)
        {

            var tableName = GetTableName(table.Name);

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ")
                .Append(tableName);
            
            if (table.Columns.Any())
            {
                sb.Append("(");
                sb.Append(System.Environment.NewLine);

                var i = 0;
                var primaryKey = string.Empty;
                foreach (var column in table.Columns)
                {
                    if (column.PrimaryKey)
                        primaryKey = column.Name;
                    sb.Append(DescribeTableColumn(column));
                    if (!string.IsNullOrEmpty(primaryKey))
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        if (i < table.Columns.Count)
                            sb.Append(",");
                    }
                    sb.Append(System.Environment.NewLine);
                    i += 1;
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

                sb.Append(System.Environment.NewLine);
                sb.Append(");");
            
            }

            sb.Append("SELECT 1 AS MigrationId;");

            CreateStatement(sb.ToString());

            return this;
        }


        public ISchemaBuilder DropTable(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            sb.Append("DROP TABLE ")
                .Append(tableName);
            CreateStatement(sb.ToString());
            return this;
        }
        
        public ISchemaBuilder AlterTableColumns(SchemaTable table)
        {

            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            if (table.Columns.Any())
            {
                foreach (var column in table.Columns)
                {
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ADD ")
                        .Append(DescribeTableColumn(column))
                        .Append(";")
                        .Append(System.Environment.NewLine);
                }
              
                sb.Append("SELECT 1 AS MigrationId;");

                CreateStatement(sb.ToString());

            }
            return this;
        }

        public ISchemaBuilder DropTableColumns(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            if (table.Columns.Any())
            {
                foreach (var column in table.Columns)
                {
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" DROP ")
                        .Append(column.Name)
                        .Append(";");
                }

                CreateStatement(sb.ToString());

            }
            return this;
        }

        public ISchemaBuilder CreateStoredProcedure(SchemaStoredProcedure procedure)
        {
          
            var statement = string.Empty;
            switch (procedure.ProcedureType)
            {
                case StoredProcedureType.InsertUpdate:
                    statement = BuildInsertUpdateProcedure(procedure);
                    break;
                case StoredProcedureType.Select:
                    statement = BuildSelectProcedure(procedure);
                    break;
            }

            CreateStatement(statement);

            return this;

        }

        public ISchemaBuilder CreateStatement(string statement)
        {
            var notNull = !string.IsNullOrEmpty(statement);
            var notPresent = !_statements.Contains(statement);
            if (notNull && notPresent)
                _statements.Add(statement);
            return this;
        }
        
        public ISchemaBuilder Apply()
        {

            using (var context = _dbContext)
            {
                foreach (var statement in _statements)
                {
                    try
                    {
                        context.Execute(CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        _errors.Add(ex);
                    }
                }

            }


            return this;
        }

        public void Dispose()
        {
            _statements = null;
            _errors = null;
        }

        #endregion

        #region "Private Methods"

        private string BuildSelectProcedure(SchemaStoredProcedure procedure)
        {
            return $"CREATE PROCEDURE [{GetProcedureName(procedure.Name)}] AS SET NOCOUNT ON SELECT * FROM {GetTableName(procedure.Table.Name)} WITH (nolock)";
        }


        private string BuildInsertUpdateProcedure(SchemaStoredProcedure procedure)
        {

            var tableName = GetTableName(procedure.Table.Name);
            var columns = procedure.Table.Columns;
            if (procedure.Table.PrimaryKeyColumn == null)
                throw new Exception($"A primary key column is required for table '{procedure.Table.Name}' when creating procedure of type '{procedure.ProcedureType}'");

            var sb = new StringBuilder();

            sb.Append("CREATE PROCEDURE [")
                .Append(GetProcedureName(procedure.Name))
                .Append("]");

            if (columns.Any())
            {
                sb.Append("(")
                    .Append(Environment.NewLine);

                foreach (var column in columns)
                {
                    sb
                        .Append("     ")
                        .Append("@")
                        .Append(column.ParameterizedName)
                        .Append(" ")
                        .Append(column.DbTypeNormalized)
                        .Append(",")
                        .Append(Environment.NewLine);
                }

                sb
                    .Append("     ")
                    .Append("@unique_id int output")
                    .Append(Environment.NewLine)
                    .Append(")")
                    .Append(Environment.NewLine);

            }

            sb.Append("AS")
                .Append(Environment.NewLine)
                .Append(Environment.NewLine)
                .Append("SET NOCOUNT ON ")
                .Append(Environment.NewLine);
             
            sb.Append("IF EXISTS (SELECT ")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" FROM ")
                .Append(tableName)
                .Append(" WHERE ")
                .Append("(")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" = ")
                .Append("@")
                .Append(procedure.Table.PrimaryKeyColumn?.ParameterizedName)
                .Append("))")
                .Append(Environment.NewLine);

            // update

            sb.Append("BEGIN")
                .Append(Environment.NewLine);
            
            if (columns.Any())
            {
                sb
                    .Append("   ")
                    .Append("UPDATE ")
                    .Append(tableName)
                    .Append(" SET ")
                    .Append(Environment.NewLine);
                var i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append(column.Name)
                            .Append(" = ")
                            .Append("@")
                            .Append(column.ParameterizedName)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(Environment.NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("       ")
                    .Append("WHERE ")
                    .Append(procedure.Table.PrimaryKeyColumn.Name)
                    .Append(" = ")
                    .Append("@")
                    .Append(procedure.Table.PrimaryKeyColumn.ParameterizedName)
                    .Append(Environment.NewLine);
            }

            sb
                .Append(Environment.NewLine)
                .Append("     ")
                .Append("SET @unique_id = @")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(";")
                .Append(Environment.NewLine)
                .Append(Environment.NewLine);

            sb
                .Append("END")
                .Append(Environment.NewLine)
                .Append("ELSE")
                .Append(Environment.NewLine)
                .Append("BEGIN")
                .Append(Environment.NewLine);

            if (columns.Any())
            {
                sb
                    .Append("   ")
                    .Append("INSERT INTO ")
                    .Append(tableName)
                    .Append(" ( ")
                    .Append(Environment.NewLine);
                var i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append(column.Name)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(Environment.NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("   ")
                    .Append(") VALUES (")
                    .Append(Environment.NewLine);
                i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append("@")
                            .Append(column.ParameterizedName)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(Environment.NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("     ")
                    .Append(")");

                sb
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("     ")
                    .Append("SET @unique_id = SCOPE_IDENTITY();")
                    .Append(Environment.NewLine);

            }
            
            sb.Append("END")
                .Append(Environment.NewLine);
            
            return sb.ToString();

        }
        
        private string GetTableName(string tableName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + "_" + tableName
                : tableName;
        }

        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + "_" + procedureName
                : procedureName;
        }

        private string DescribeTableColumn(
            SchemaColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(column.Name).Append(" ").Append(column.DbTypeNormalized);

            if (column.PrimaryKey)
            {
                sb.Append(" IDENTITY(1,1)");
            }
            else
            {
                if (!string.IsNullOrEmpty(column.DefaultValueNormalizsed))
                    sb.Append(" DEFAULT (").Append(column.DefaultValueNormalizsed).Append(")");
            }
             
            sb.Append(column.Nullable ? " NULL" : " NOT NULL");
            return sb.ToString();
        }

        #endregion
        

    }
}
