using System;
using System.Collections.Generic;
using System.Data;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private readonly IDataMigrationBuilder _migrationBuilder;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IDataMigrationManager _migrationManager;

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder,
            IDataMigrationManager migrationManager,
            ISchemaBuilder schemaBuilder)
        {
            _migrationBuilder = migrationBuilder;
            _schemaBuilder = schemaBuilder;
            _migrationManager = migrationManager;
        }

        #endregion

        #region "Private Methods"

        public DataMigrationResult InitialMigration()
        {

            using (var builder = _schemaBuilder)
            {

                builder.Configure(options =>
                {
                    options.ModuleName = "Plato.Core";
                    options.Version = "1.0.0";
                });

                // Settings
                BuildSettings(builder);

                // Users
                BuildUsers(builder);

                var result = builder.Apply();
                return new DataMigrationResult()
                {
                    Errors = result.Errors
                };
            }
            
            //return _migrationBuilder.BuildMigrations(
            //    new List<string>()
            //    {
            //        "1.0.0"
            //    }).ApplyMigrations();

        }


        private void BuildSettings(ISchemaBuilder builder)
        {

            var table = new SchemaTable()
            {
                Name = "Settings",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };

            builder
                .CreateTable(table)
                .CreateDefaultProcedures(table)
                .CreateProcedure(new SchemaProcedure("SelectSettingsByKey", StoredProcedureType.SelectByKey)
                    .ForTable(table).WithKey(new SchemaColumn() {Name = "[Key]", DbType = DbType.Int32}));

        }

        private void BuildUsers(ISchemaBuilder builder)
        {

            var table = new SchemaTable()
            {
                Name = "Users",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedUserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedEmail",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "DisplayName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SamAccountName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordHash",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityStamp",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumber",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumberConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TwoFactorEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnd",
                        Nullable = true,
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "AccessFailedCount",
                        DbType = DbType.Int32
                    },
                }
            };
            
            builder
                .CreateTable(table)
                .CreateDefaultProcedures(table)
                .CreateProcedure(new SchemaProcedure("SelectUserByEmail", StoredProcedureType.SelectByKey)
                    .ForTable(table).WithKey(new SchemaColumn() { Name = "Email", DbType = DbType.String, Length = "255" }))
                .CreateProcedure(new SchemaProcedure("SelectUserByUserName", StoredProcedureType.SelectByKey)
                    .ForTable(table).WithKey(new SchemaColumn() { Name = "Username", DbType = DbType.String, Length = "255" }))
                .CreateProcedure(new SchemaProcedure("SelectUserByUserNameNormalized", StoredProcedureType.SelectByKey)
                    .ForTable(table).WithKey(new SchemaColumn() { Name = "NormalizedUserName", DbType = DbType.String, Length = "255" }))
                .CreateProcedure(new SchemaProcedure("SelectUserByEmailAndPassword", StoredProcedureType.SelectByKey)
                    .ForTable(table).WithKey(new SchemaColumn() { Name = "NormalizedUserName", DbType = DbType.String, Length = "255" }));


            





        }

        #endregion




    }
}
