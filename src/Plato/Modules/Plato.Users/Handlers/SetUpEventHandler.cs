using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IUserColorProvider _userColorProvider;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private readonly SchemaTable _users = new SchemaTable()
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
                        Name = "PrimaryRoleId",
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
                        Name = "FirstName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "LastName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoUrl",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoColor",
                        Length = "6",
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
                        Name = "PasswordExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
                        DbType = DbType.DateTimeOffset
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
                    new SchemaColumn()
                    {
                        Name = "ResetToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ConfirmationToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ApiKey",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "TimeZone",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ObserveDst",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "Culture",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Theme",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Visits",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "VisitsUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActive",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActiveUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "Reputation",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ReputationUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                     new SchemaColumn()
                    {
                        Name = "[Rank]",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RankUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "[Signature]",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerified",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBanned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "UserType",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "LastLoginDate",
                        DbType = DbType.DateTimeOffset,
                    }
                }
        };

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            UserManager<User> userManager,
            IUserColorProvider userColorProvider,
            RoleManager<Role> roleManager)
        {
            _schemaBuilder = schemaBuilder;
            _userManager = userManager;
            _userColorProvider = userColorProvider;
            _roleManager = roleManager;
        }

        #region "Implementation"

        public override async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            // Build schemas
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // user schema
                Users(builder);

                // photo schema
                UserPhoto(builder);

                // banner schema
                UserBanner(builder);

                // meta aata schema
                UserData(builder);
                
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }

                }

            }

            // Configure default users
            await ConfigureDefaultUsers(context, reportError);

            // Configure administrator
            await ConfigureSuperUser(context, reportError);

        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = base.ModuleId;
                    options.Version = "1.0.0";
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Users(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_users)
                .CreateDefaultProcedures(_users)

                // Overwrite our SelectEntityById created via CreateDefaultProcedures
                // above to also return all EntityData within a second result set
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectUserById",
                            @"
                                /* user */
                                SELECT * FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Id = @Id
                                )
                                /* user data */
                                SELECT * FROM {prefix}_UserData WITH (nolock) 
                                WHERE (
                                   UserId = @Id
                                )
                                /* roles */
                                SELECT r.* FROM {prefix}_UserRoles ur WITH (nolock) 
                                INNER JOIN {prefix}_Roles r ON ur.RoleId = r.Id 
                                WHERE (
                                    ur.UserId = @Id
                                )")
                        .ForTable(_users)
                        .WithParameter(_users.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectUserByEmail", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "Email", DbType = DbType.String, Length = "255"}))


                .CreateProcedure(new SchemaProcedure("SelectUserByEmailNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedEmail = @NormalizedEmail
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(
                        new SchemaColumn() {Name = "NormalizedEmail", DbType = DbType.String, Length = "255"}))


                .CreateProcedure(new SchemaProcedure("SelectUserByUserName", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   UserName = @UserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "UserName", DbType = DbType.String, Length = "255"}))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByUserNameNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedUserName = @NormalizedUserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "NormalizedUserName",
                            DbType = DbType.String,
                            Length = "255"
                        }))


                .CreateProcedure(new SchemaProcedure("SelectUserByApiKey", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ApiKey = @ApiKey
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "ApiKey", DbType = DbType.String, Length = "255"}))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByEmailAndPassword", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email AND PasswordHash = @PasswordHash
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
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

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByResetToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ResetToken = @ResetToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ResetToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByConfirmationToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ConfirmationToken = @ConfirmationToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ConfirmationToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(new SchemaProcedure("SelectUsersPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_users)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "RoleName",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }

        void UserPhoto(ISchemaBuilder builder)
        {

            var userPhoto = new SchemaTable()
            {
                Name = "UserPhoto",
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
                        Name = "[Name]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentBlob",
                        Nullable = true,
                        DbType = DbType.Binary
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentType",
                        Length = "75",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentLength",
                        DbType = DbType.Int64
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                   
                }
            };

            builder
                // Create tables
                .CreateTable(userPhoto)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(userPhoto)
                .CreateProcedure(new SchemaProcedure("SelectUserPhotoByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userPhoto)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}));

        }

        void UserBanner(ISchemaBuilder builder)
        {

            var userBanner = new SchemaTable()
            {
                Name = "UserBanner",
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
                        Name = "[Name]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentBlob",
                        Nullable = true,
                        DbType = DbType.Binary
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentType",
                        Length = "75",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentLength",
                        DbType = DbType.Int64
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }

                }
            };

            builder
                // Create tables
                .CreateTable(userBanner)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(userBanner)
                .CreateProcedure(new SchemaProcedure("SelectUserBannerByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userBanner)
                    .WithParameter(new SchemaColumn() { Name = "UserId", DbType = DbType.Int32 }));

        }
        
        void UserData(ISchemaBuilder builder)
        {

            var userData = new SchemaTable()
            {
                Name = "UserData",
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
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
            };

            builder
                .CreateTable(userData)
                .CreateDefaultProcedures(userData)
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userData)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}))
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByKeyAndUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }
                    }));
            
            builder.CreateProcedure(new SchemaProcedure("SelectUserDatumPaged", StoredProcedureType.SelectPaged)
                .ForTable(userData)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));


        }

        async Task ConfigureDefaultUsers(SetUpContext context, Action<string, string> reportError)
        {

            try
            {

                // create super user
                var result1 = await _userManager.CreateAsync(new User()
                {
                    Email = context.AdminEmail,
                    UserName = context.AdminUsername,
                    PhotoColor = _userColorProvider.GetColor(),
                    EmailConfirmed = true,
                    IsVerified = true,
                    IsVerifiedUpdatedUserId = 1,
                    IsVerifiedUpdatedDate = DateTimeOffset.UtcNow
                }, context.AdminPassword);
                if (!result1.Succeeded)
                {
                    foreach (var error in result1.Errors)
                    {
                        reportError(error.Code, error.Description);
                    }
                }

                // create default plato bot

                var result2 = await _userManager.CreateAsync(new User()
                {
                    Email = "bot@plato.com",
                    UserName = "PlatoBot",
                    DisplayName = "Plato Bot",
                    EmailConfirmed = true,
                    IsVerified = true,
                    IsVerifiedUpdatedUserId = 1,
                    IsVerifiedUpdatedDate = DateTimeOffset.UtcNow,
                    PhotoUrl = "/images/bot.png",
                    PhotoColor = _userColorProvider.GetColor(),
                    UserType = UserType.Bot
                }, context.AdminPassword);
                if (!result2.Succeeded)
                {
                    foreach (var error in result2.Errors)
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

        async Task ConfigureSuperUser(SetUpContext context, Action<string, string> reportError)
        {

            // Get newly installed administrator role
            var role = await _roleManager.FindByNameAsync(DefaultRoles.Administrator);

            // Get newly created administrator user
            var user = await _userManager.FindByNameAsync(context.AdminUsername);

            // Add our administrator user to the administrator role
            var dirty = false;
            if (role != null && user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                    dirty = true;
                }

            }

            if (dirty)
            {
                await _userManager.UpdateAsync(user);
            }

        }


        #endregion

    }

}
