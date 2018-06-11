using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Roles.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly UserManager<User> _userManager;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            UserManager<User> userManager)
        {
            _schemaBuilder = schemaBuilder;
            _userManager = userManager;
        }

        public async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            // build schemas

            var roles = new SchemaTable()
            {
                Name = "Roles",
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
                        Name = "[ViewName]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Description",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Claims",
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ConcurrencyStamp",
                        Length = "255",
                        DbType = DbType.String
                    }
                }
            };


            var userRoles = new SchemaTable()
            {
                Name = "UserRoles",
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
                        Name = "[ViewName]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RoleId",
                        DbType = DbType.Int32
                    }
                }
            };


            using (var builder = _schemaBuilder)
            {

                // create tables and default procedures
                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Roles";
                        options.Version = "1.0.0";
                    })
                    // Create tables
                    .CreateTable(roles)
                    .CreateTable(userRoles)
                    // Create basic default CRUD procedures
                    .CreateDefaultProcedures(roles)
                    .CreateDefaultProcedures(userRoles);

                // create unique role stored procedures

                builder.CreateProcedure(
                    new SchemaProcedure("SelectRolesByUserId", StoredProcedureType.SelectByKey)
                        .ForTable(userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))
                .CreateProcedure(
                    new SchemaProcedure("DeleteRolesByUserId", StoredProcedureType.DeleteByKey)
                        .ForTable(userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }));


                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }

                }

            }

            // create super user

            try
            {
             
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }

        }

    }
}
