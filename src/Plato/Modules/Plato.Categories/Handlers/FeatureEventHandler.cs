using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Categories.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
    
        public string Version { get; } = "1.0.0";
        
        // Categories table
        private readonly SchemaTable _categories = new SchemaTable()
        {
            Name = "Categories",
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
                        Name = "ParentId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "FeatureId",
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
                        Name = "[Description]",
                        Length = "500",
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
                        Name = "IconCss",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ForeColor",
                        Length = "50",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "BackColor",
                        Length = "50",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime
                    }
                }
        };

        // Category data table
        private readonly SchemaTable _categoryData = new SchemaTable()
        {
            Name = "CategoryData",
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
                    Name = "CategoryId",
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

        // Category Roles table
        private readonly SchemaTable _categoryRoles = new SchemaTable()
        {
            Name = "CategoryRoles",
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
                        Name = "CategoryId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RoleId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime
                    }
                }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }
        
        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");
            
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Categories schema
                Categories(builder);

                // Category data
                CategoryData(builder);

                // Category roles schema
                CategoryRoles(builder);
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Errors.Add(error.Message, $"InstallingAsync within {this.GetType().FullName}");
                    }
                }

            }
            
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");
            
            using (var builder = _schemaBuilder)
            {
                
                // drop categories
                builder
                    .DropTable(_categories)
                    .DropDefaultProcedures(_categories)
                    .DropProcedure(new SchemaProcedure("SelectCategoriesPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectCategoriesByFeatureId", StoredProcedureType.SelectByKey));
                
                // drop category data
                builder
                    .DropTable(_categoryData)
                    .DropDefaultProcedures(_categoryData)
                    .DropProcedure(new SchemaProcedure("SelectCategoryDatumByCategoryId"))
                    .DropProcedure(new SchemaProcedure("SelectCategoryDatumPaged"));
                
                // drop category roles
                builder
                    .DropTable(_categoryRoles)
                    .DropDefaultProcedures(_categoryRoles)
                    .DropProcedure(new SchemaProcedure("SelectCategoryRolesByCategoryId"))
                    .DropProcedure(new SchemaProcedure("SelectCategoryRolesPaged"))
                    .DropProcedure(new SchemaProcedure("DeleteCategoryRolesByCategoryId"));


                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Logger.LogCritical(error.Message, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                        context.Errors.Add(error.Message, $"UninstallingAsync within {this.GetType().FullName}");
                    }
                }

            }
            
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {

            return Task.CompletedTask;
        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Categories(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_categories)
                .CreateDefaultProcedures(_categories);

            builder
                .CreateProcedure(new SchemaProcedure("SelectCategoriesByFeatureId", StoredProcedureType.SelectByKey)
                    .ForTable(_categories)
                    .WithParameter(new SchemaColumn() {Name = "FeatureId", DbType = DbType.Int32}));
            
            builder.CreateProcedure(new SchemaProcedure("SelectCategoriesPaged", StoredProcedureType.SelectPaged)
                .ForTable(_categories)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "Keywords",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));

        }

        void CategoryData(ISchemaBuilder builder)
        {

            builder
                // Create tables
                .CreateTable(_categoryData)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(_categoryData)
                .CreateProcedure(new SchemaProcedure("SelectCategoryDatumBycategoryId", StoredProcedureType.SelectByKey)
                    .ForTable(_categoryData)
                    .WithParameter(new SchemaColumn() { Name = "CategoryId", DbType = DbType.Int32 }));

            builder.CreateProcedure(new SchemaProcedure("SelectCategoryDatumPaged", StoredProcedureType.SelectPaged)
                .ForTable(_categoryData)
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


        void CategoryRoles(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_categoryRoles)
                .CreateDefaultProcedures(_categoryRoles);

            builder
                .CreateProcedure(new SchemaProcedure("SelectCategoryRolesByCategoryId", StoredProcedureType.SelectByKey)
                    .ForTable(_categories)
                    .WithParameter(new SchemaColumn() { Name = "CategoryId", DbType = DbType.Int32 }));

            builder
                .CreateProcedure(new SchemaProcedure("DeleteCategoryRolesByCategoryId", StoredProcedureType.DeleteByKey)
                    .ForTable(_categories)
                    .WithParameter(new SchemaColumn() { Name = "CategoryId", DbType = DbType.Int32 }));
            
            builder.CreateProcedure(new SchemaProcedure("SelectCategoryRolesPaged", StoredProcedureType.SelectPaged)
                .ForTable(_categoryRoles)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "Keywords",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));


        }

        #endregion

    }

}
