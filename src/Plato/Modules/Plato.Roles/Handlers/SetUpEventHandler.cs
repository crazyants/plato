using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Roles.Handlers
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

            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // roles 
                Roles(builder);

                // user roles 
                UserRoles(builder);
                
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }

                }

            }

        }
        
        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = "Plato.Roles";
                    options.Version = "1.0.0";
                });

        }

        void Roles(ISchemaBuilder builder)
        {

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
                        Name = "[Name]",
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

            // create tables and default procedures
            builder
                // Create tables
                .CreateTable(roles)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(roles)

            // create unique stored procedures

                .CreateProcedure(
                    new SchemaProcedure("SelectRoleByName", StoredProcedureType.SelectByKey)
                        .ForTable(roles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "[Name]",
                            DbType = DbType.String,
                            Length = "255"
                        }))
                .CreateProcedure(
                    new SchemaProcedure("SelectRoleByNameNormalized", StoredProcedureType.SelectByKey)
                        .ForTable(roles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "NormalizedName",
                            DbType = DbType.String,
                            Length = "255"
                        }))
                .CreateProcedure(new SchemaProcedure("SelectRolesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(roles)
                    .WithParameters(new List<SchemaColumn>()
                    {
                            new SchemaColumn()
                            {
                                Name = "Id",
                                DbType = DbType.Int32
                            },
                            new SchemaColumn()
                            {
                                Name = "[Name]",
                                DbType = DbType.String,
                                Length = "255"
                            }
                    }));
            
        }

        void UserRoles(ISchemaBuilder builder)
        {

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
            
            // create tables and default procedures
            builder
                // Create tables
                .CreateTable(userRoles)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(userRoles);

            builder

                .CreateProcedure(
                    new SchemaProcedure("SelectRolesByUserId", @"
                            SELECT * FROM {prefix}_Roles WITH (nolock) WHERE Id IN (
	                            SELECT RoleId FROM {prefix}_UserRoles WITH (nolock) 
	                            WHERE (
	                               UserId = @UserId
	                            )
                            )
                        ")
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))
                        
                .CreateProcedure(
                    new SchemaProcedure("SelectUserRolesByUserId", StoredProcedureType.SelectByKey)
                        .ForTable(userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))

                .CreateProcedure(
                    new SchemaProcedure("DeleteUserRolesByUserId", StoredProcedureType.DeleteByKey)
                        .ForTable(userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))
                .CreateProcedure(
                    new SchemaProcedure("DeleteUserRoleByUserIdAndRoleId", StoredProcedureType.DeleteByKey)
                        .ForTable(userRoles)
                        .WithParameters(new List<SchemaColumn>()
                        {
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
                        }));
            
        }
        
    }
}
