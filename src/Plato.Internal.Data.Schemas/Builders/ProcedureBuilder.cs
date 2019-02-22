using System;
using System.Text;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions.Builders;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Data.Schemas.Builders
{
    public class ProcedureBuilder : SchemaBuilderBase, IProcedureBuilder
    {

        public ProcedureBuilder(
            IDbContext dbContext,
            IPluralize pluralize) : base(dbContext, pluralize)
        {
        }

        public virtual IProcedureBuilder CreateProcedure(SchemaProcedure procedure)
        {
            if (Options.DropProceduresBeforeCreate)
            {
                DropProcedure(procedure);
            }

            AddStatement(GetProcedureStatement(procedure, false));
            return this;
        }

        public virtual IProcedureBuilder DropProcedure(SchemaProcedure procedure)
        {
            var sb = new StringBuilder();

            sb.Append("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'")
                .Append(GetProcedureName(procedure.Name))
                .Append("') AND type in (N'P', N'PC'))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append("DROP PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(";")
                .Append(NewLine)
                .Append("END");

            AddStatement(sb.ToString());
            return this;

        }

        public virtual IProcedureBuilder AlterProcedure(SchemaProcedure procedure)
        {
            AddStatement(GetProcedureStatement(procedure, true));
            return this;
        }

        public virtual IProcedureBuilder CreateDefaultProcedures(SchemaTable table)
        {
            // select * from table
            CreateProcedure(new SchemaProcedure($"Select{table.NameNormalized}", StoredProcedureType.Select)
                .ForTable(table));

            // select * from table where primaryKey = @primaryKey
            CreateProcedure(
                new SchemaProcedure(
                        $"Select{GetSingularTableName(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.SelectByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // delete from table where primaryKey = @primaryKey
            CreateProcedure(
                new SchemaProcedure(
                        $"Delete{GetSingularTableName(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // insert / update by primary key
            CreateProcedure(
                new SchemaProcedure($"InsertUpdate{GetSingularTableName(table.Name)}",
                        StoredProcedureType.InsertUpdate)
                    .ForTable(table));

            return this;

        }

        public virtual IProcedureBuilder DropDefaultProcedures(SchemaTable table)
        {

            DropProcedure(new SchemaProcedure($"Select{table.NameNormalized}", StoredProcedureType.Select)
                .ForTable(table));

            DropProcedure(
                new SchemaProcedure(
                        $"Select{GetSingularTableName(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.SelectByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // delete from table where primaryKey = @primaryKey
            DropProcedure(
                new SchemaProcedure(
                        $"Delete{GetSingularTableName(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // insert / update by primary key
            DropProcedure(
                new SchemaProcedure($"InsertUpdate{GetSingularTableName(table.Name)}",
                        StoredProcedureType.InsertUpdate)
                    .ForTable(table));

            return this;

        }

        // -----------------

        private string GetProcedureStatement(SchemaProcedure procedure, bool alter)
        {

            // Always return explicit SQL first
            if (!String.IsNullOrEmpty(procedure.Sql))
            {
                return BuildExplicitProcedure(procedure, alter);
            }

            // TODO: Refactor to avoid switch statement 
            // Create common command classes to represent each procedure type
            // Avoids enum and makes it more extensible for future addition
            // InsertUpdateProcedure : IProcedureCommand,
            // SelectByKeyProcedure : IProcedureCommand
            // DeleteByKey : IProcedureCommand etc

            switch (procedure.ProcedureType)
            {
                case StoredProcedureType.InsertUpdate:
                    return BuildInsertUpdateProcedure(procedure, alter);
                case StoredProcedureType.Select:
                    return BuildSelectProcedure(procedure, alter);
                case StoredProcedureType.SelectByKey:
                    return BuildSelectByProcedure(procedure, alter);
                case StoredProcedureType.SelectPaged:
                    return BuildSelectPagedProcedure(procedure, alter);
                case StoredProcedureType.DeleteByKey:
                    return BuildDeleteByKeyProcedure(procedure, alter);
            }

            return string.Empty;

        }

        private string BuildSelectProcedure(SchemaProcedure procedure, bool alter)
        {

            var sb = new StringBuilder();

            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE ")
               .Append(GetProcedureName(procedure.Name))
                .Append(NewLine)
                .Append("AS")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("SET NOCOUNT ON")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("SELECT * FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(" WITH (nolock)");

            return sb.ToString();

        }

        private string BuildSelectByProcedure(SchemaProcedure procedure, bool alter)
        {

            if (procedure.Parameters == null)
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");

            var sb = new StringBuilder();
            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(NewLine);

            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(NewLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(NewLine)
                .Append("SET NOCOUNT ON")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("SELECT * FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(" WITH (nolock) ")
                .Append(NewLine)
                .Append("WHERE (")
                .Append(NewLine);

            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append(parameter.Name)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? " AND " : "")
                    .Append(NewLine);
                i += 1;
            }

            sb.Append(")");

            return sb.ToString();

        }

        private string BuildSelectPagedProcedure(SchemaProcedure procedure, bool alter)
        {

            /* Generates a stored procedure similar to...
                         
            CREATE PROCEDURE [SelectoSomeData] (
               @PageIndex int = 1,
               @PageSize int = 10,              
               @SqlPopulate nvarchar(max),
               @SqlCount nvarchar(max),
               @Name nvarchar(255),
               @Description nvarchar(255)
            ) AS
            SET NOCOUNT ON

            DECLARE @RowIndex int = 0;
            IF(@PageIndex > 1)
            SET @RowIndex = ((@PageIndex * @PageSize) - (@PageSize))

            DECLARE @Params nvarchar(max) = '@Name nvarchar(255), @Description nvarchar(255)';
            SET @Params = '@RowIndex int,@PageSize int,' + @Params;

            EXECUTE sp_executesql @SqlPopulate, @Params,
                @RowIndex = @RowIndex,
                @PageSize = @PageSize,
                @Name = @Name,
                @Description = @Description;

            --total count
            IF(@SqlCount <> '')
            BEGIN            
                EXECUTE sp_executesql @SqlCount, @Params,  
                    @RowIndex = @RowIndex,
                    @PageSize = @PageSize,
                    @Name = @Name,
                    @Description = @Description;
            END

            */

            if (procedure.Parameters == null)
            {
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");
            }

            var sb = new StringBuilder();
            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE ")
                  .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(NewLine);

            sb.Append("   ")
                .Append("@PageIndex int = 1,")
                .Append(NewLine);
            sb.Append("   ")
                .Append("@PageSize int = 10,")
                .Append(NewLine);
            sb.Append("   ")
                .Append("@SqlPopulate nvarchar(max),")
                .Append(NewLine);
            sb.Append("   ")
                .Append("@SqlCount nvarchar(max),")
                .Append(NewLine);
            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(NewLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(NewLine)
                .Append("SET NOCOUNT ON")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("DECLARE @RowIndex int = 0;")
                .Append(NewLine)
                .Append("IF (@PageIndex > 1)")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append("   ")
                .Append("SET @RowIndex = ((@PageIndex * @PageSize) - (@PageSize));")
                .Append(NewLine)
                .Append("END")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("DECLARE @Params nvarchar(max) = '@RowIndex int,@PageSize int");
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                if (i == 0)
                {
                    sb.Append(",");
                }
                sb.Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "");
                i += 1;
            }

            sb.Append("';")
                .Append(NewLine)
                .Append(NewLine);

            sb
                .Append("-- get paged data")
                .Append(NewLine)
                .Append("EXECUTE sp_executesql @SqlPopulate, @Params,")
                .Append(NewLine)
                .Append("   ")
                .Append("@RowIndex = @RowIndex,")
                .Append(NewLine)
                .Append("   ")
                .Append("@PageSize = @PageSize,")
                .Append(NewLine);
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : ";")
                    .Append(NewLine);
                i += 1;
            }

            sb
                .Append(NewLine)
                .Append("-- get total count")
                .Append(NewLine)
                .Append("IF (@SqlCount <> '')")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append("   ")
                .Append("EXECUTE sp_executesql @SqlCount, @Params,")
                .Append(NewLine)
                .Append("       ")
                .Append("@RowIndex = @RowIndex,")
                .Append(NewLine)
                .Append("       ")
                .Append("@PageSize = @PageSize,")
                .Append(NewLine);
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("       ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : ";")
                    .Append(NewLine);
                i += 1;
            }

            sb.Append("END");

            return sb.ToString();

        }

        private string BuildDeleteByKeyProcedure(SchemaProcedure procedure, bool alter)
        {

            if (procedure.Parameters == null)
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");

            var sb = new StringBuilder();
            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE ")
               .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(NewLine);

            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(NewLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(NewLine)
                .Append("SET NOCOUNT ON")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            // ensure exists 

            sb.Append("IF EXISTS (SELECT ")
                .Append(procedure.Table.PrimaryKeyColumn.NameNormalized)
                .Append(" FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(" WHERE (");

            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append(parameter.Name)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? " AND " : "");
                i += 1;
            }

            sb.Append("))")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append(NewLine);

            // perform delete

            sb
                .Append("   ")
                .Append("DELETE FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(NewLine)
                .Append("   ")
                .Append("WHERE (")
                .Append(NewLine);

            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("       ")
                    .Append(parameter.Name)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? " AND " : "")
                    .Append(NewLine);
                i += 1;
            }

            sb
                .Append("   ")
                .Append(")")
                .Append(NewLine)
                .Append(NewLine); // end DELETE where

            // We found the entry return success
            sb
                .Append("   ")
                .Append("SELECT 1;")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("END")
                .Append(NewLine)
                .Append(NewLine); // end EXISTS check

            // The entry could not be found
            sb.Append("SELECT 0;");

            return sb.ToString();

        }

        private string BuildExplicitProcedure(SchemaProcedure procedure, bool alter)
        {

            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            var parameters = procedure.Parameters;

            var sb = new StringBuilder();

            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE [")
                 .Append(GetProcedureName(procedure.Name))
                .Append("]");

            if (parameters.Count > 0)
            {
                sb.Append("(")
                    .Append(NewLine);

                var i = 0;
                foreach (var parameter in parameters)
                {
                    sb
                        .Append("     ")
                        .Append("@")
                        .Append(parameter.NameNormalized)
                        .Append(" ")
                        .Append(parameter.DbTypeNormalized)
                        .Append(parameter.Direction == Direction.Out ? " output" : "")
                        .Append(i < parameters.Count - 1 ? "," : "")
                        .Append(NewLine);
                    i += 1;
                }

                sb.Append(") ");

            }

            sb.Append("AS")
                .Append(NewLine)
                .Append(NewLine)
                .Append("SET NOCOUNT ON ")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(ParseExplicitTSql(procedure.Sql));

            return sb.ToString();

        }

        private string BuildInsertUpdateProcedure(SchemaProcedure procedure, bool alter)
        {

            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            if (procedure.Table == null)
                throw new ArgumentNullException(nameof(procedure.Table));

            if (procedure.Table.PrimaryKeyColumn == null)
                throw new Exception($"A primary key column is required for table '{procedure.Table.Name}' when creating procedure of type '{procedure.ProcedureType}'");

            var tableName = GetTableName(procedure.Table.Name);
            var columns = procedure.Table.Columns;

            // Check to ensure we have more than just a primary key
            var hasColumns = false;
            foreach (var column in columns)
            {
                if (!column.PrimaryKey)
                {
                    hasColumns = true;
                }
            }

            // We need columns to insert or update
            if (!hasColumns)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.Append(alter == false ? "CREATE" : "ALTER")
                .Append(" PROCEDURE [")
                .Append(GetProcedureName(procedure.Name))
                .Append("]");

            if (columns.Count > 0)
            {
                sb.Append("(")
                    .Append(NewLine);

                var i = 0;
                foreach (var column in columns)
                {
                    sb
                        .Append("     ")
                        .Append("@")
                        .Append(column.NameNormalized)
                        .Append(" ")
                        .Append(column.DbTypeNormalized)
                        .Append(column.Direction == Direction.Out ? " output" : "")
                        .Append(",")
                        .Append(NewLine);
                    i += 1;
                }

                sb
                    .Append("     ")
                    .Append("@UniqueId int = 0 output")
                    .Append(NewLine);

                sb.Append(") ");

            }

            sb.Append("AS")
                .Append(NewLine)
                .Append(NewLine)
                .Append("SET NOCOUNT ON ")
                .Append(NewLine)
                .Append(NewLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(NewLine)
                .Append(NewLine);

            sb.Append("IF EXISTS (SELECT ")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" FROM ")
                .Append(tableName)
                .Append(" WHERE ")
                .Append("(")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" = ")
                .Append("@")
                .Append(procedure.Table.PrimaryKeyColumn.NameNormalized)
                .Append("))")
                .Append(NewLine);

            // update

            sb.Append("BEGIN")
                .Append(NewLine)
                .Append(NewLine);

            if (columns.Count > 0)
            {
                sb
                    .Append("   ")
                    .Append("UPDATE ")
                    .Append(tableName)
                    .Append(" SET ")
                    .Append(NewLine);
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
                            .Append(column.NameNormalized)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("       ")
                    .Append("WHERE ")
                    .Append(procedure.Table.PrimaryKeyColumn.Name)
                    .Append(" = ")
                    .Append("@")
                    .Append(procedure.Table.PrimaryKeyColumn.NameNormalized)
                    .Append(NewLine);
            }

            sb
                .Append(NewLine)
                .Append("     ")
                .Append("SET @UniqueId = @")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(";")
                .Append(NewLine)
                .Append(NewLine);

            sb
                .Append("END")
                .Append(NewLine)
                .Append("ELSE")
                .Append(NewLine)
                .Append("BEGIN")
                .Append(NewLine)
                .Append(NewLine);

            if (columns.Count > 0)
            {
                sb
                    .Append("   ")
                    .Append("INSERT INTO ")
                    .Append(tableName)
                    .Append(" ( ")
                    .Append(NewLine);
                var i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append(column.Name)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("   ")
                    .Append(") VALUES (")
                    .Append(NewLine);
                i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append("@")
                            .Append(column.NameNormalized)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(NewLine);
                    }
                    i += 1;
                }

                sb
                    .Append("     ")
                    .Append(")");

                sb
                    .Append(NewLine)
                    .Append(NewLine)
                    .Append("     ")
                    .Append("SET @UniqueId = SCOPE_IDENTITY();")
                    .Append(NewLine)
                    .Append(NewLine);

            }

            sb.Append("END");

            return sb.ToString();

        }

        private string GetProcedurePlaceHolderComment()
        {
            var moduleName = !string.IsNullOrEmpty(Options.ModuleName) ?
                Options.ModuleName :
                "N/A";
            return $"/******{NewLine}Module: {moduleName}{NewLine}Version: {Options.Version}{NewLine}This stored procedure was generated programmatically by Plato on {DateTime.Now}. Changes made by hand may be lost.{NewLine}******/";
        }

    }
    
}
