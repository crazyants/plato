using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Services
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

            var users = new SchemaTable()
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
            
            using (var builder = _schemaBuilder)
            {

                // create tables and default procedures
                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Users";
                        options.Version = "1.0.0";
                    })
                    // Create tables
                    .CreateTable(users)
                    // Create basic default CRUD procedures
                    .CreateDefaultProcedures(users);
             
                // create unique user stored procedures

                builder.CreateProcedure(new SchemaProcedure("SelectUserByEmail", StoredProcedureType.SelectByKey)
                        .ForTable(users)
                        .WithParameter(new SchemaColumn() {Name = "Email", DbType = DbType.String, Length = "255"}))
                    .CreateProcedure(new SchemaProcedure("SelectUserByUserName", StoredProcedureType.SelectByKey)
                        .ForTable(users)
                        .WithParameter(new SchemaColumn() {Name = "Username", DbType = DbType.String, Length = "255"}))
                    .CreateProcedure(
                        new SchemaProcedure("SelectUserByUserNameNormalized", StoredProcedureType.SelectByKey)
                            .ForTable(users)
                            .WithParameter(new SchemaColumn()
                            {
                                Name = "NormalizedUserName",
                                DbType = DbType.String,
                                Length = "255"
                            }))
                    .CreateProcedure(
                        new SchemaProcedure("SelectUserByEmailAndPassword", StoredProcedureType.SelectByKey)
                            .ForTable(users)
                            .WithParameters(new List<SchemaColumn>()
                            {
                                new SchemaColumn()
                                {
                                    Name = "Email",
                                    DbType = DbType.String,
                                    Length = "255"
                                },
                                new SchemaColumn()
                                {
                                    Name = "PasswordHash",
                                    DbType = DbType.String,
                                    Length = "255"
                                }
                            }))
                    .CreateProcedure(new SchemaProcedure("SelectUsersPaged", StoredProcedureType.SelectPaged)
                        .ForTable(users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "Id",
                                DbType = DbType.Int32
                            },
                            new SchemaColumn()
                            {
                                Name = "UserName",
                                DbType = DbType.String,
                                Length = "255"
                            },
                            new SchemaColumn()
                            {
                                Name = "Email",
                                DbType = DbType.String,
                                Length = "255"
                            }
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
                var result =  await _userManager.CreateAsync(new User()
                {
                    Email = context.AdminEmail,
                    UserName = context.AdminUsername
                }, context.AdminPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Code, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }

        }

    }

}
