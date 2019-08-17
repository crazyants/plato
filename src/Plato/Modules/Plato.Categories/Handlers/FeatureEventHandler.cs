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
                    DbType = DbType.Int32,
                  
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset,
                    Nullable = true
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

        // Entity Categories table
        private readonly SchemaTable _entityCategories = new SchemaTable()
        {
            Name = "EntityCategories",
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
                    Name = "EntityId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "CategoryId",
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
                    DbType = DbType.DateTimeOffset
                }
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
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

                // Entity categories schema
                EntityCategories(builder);
                
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
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
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
                builder.TableBuilder.DropTable(_categories);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_categories)
                    .DropProcedure(new SchemaProcedure("SelectCategoriesPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectCategoriesByFeatureId", StoredProcedureType.SelectByKey));
                
                // drop category data
                builder.TableBuilder.DropTable(_categoryData);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_categoryData)
                    .DropProcedure(new SchemaProcedure("SelectCategoryDatumByCategoryId"))
                    .DropProcedure(new SchemaProcedure("SelectCategoryDatumPaged"));
                
                // drop category roles
                builder.TableBuilder.DropTable(_categoryRoles);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_categoryRoles)
                    .DropProcedure(new SchemaProcedure("SelectCategoryRolesByCategoryId"))
                    .DropProcedure(new SchemaProcedure("SelectCategoryRolesPaged"))
                    .DropProcedure(new SchemaProcedure("DeleteCategoryRolesByCategoryId"))
                    .DropProcedure(new SchemaProcedure("DeleteCategoryRolesByRoleIdAndCategoryId"));
                
                // drop entity categories
                builder.TableBuilder.DropTable(_entityCategories);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityCategories)
                    .DropProcedure(new SchemaProcedure("SelectEntityCategoriesByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityCategoryByEntityIdAndCategoryId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityCategoriesByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityCategoryByEntityIdAndCategoryId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityCategoriesPaged"));
                
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
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Logger.LogCritical(error, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                    context.Errors.Add(error, $"UninstallingAsync within {this.GetType().FullName}");
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

            // tables
            builder.TableBuilder.CreateTable(_categories);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_categories)

                // Overwrite our SelectCategoryById created via CreateDefaultProcedures
                // above to also return all CategoryData within a second result set
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectCategoryById",
                            @"SELECT * FROM {prefix}_Categories WITH (nolock) 
                                WHERE (
                                   Id = @Id
                                )
                                SELECT * FROM {prefix}_CategoryData WITH (nolock) 
                                WHERE (
                                   CategoryId = @Id
                                )")
                        .ForTable(_categories)
                        .WithParameter(_categories.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectCategoriesByFeatureId", StoredProcedureType.SelectByKey)
                    .ForTable(_categories)
                    .WithParameter(new SchemaColumn() {Name = "FeatureId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("SelectCategoriesPaged", StoredProcedureType.SelectPaged)
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

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _categories.Name,
                Columns = new string[]
                {
                    "ParentId",
                    "FeatureId"
                }
            });


        }

        void CategoryData(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_categoryData);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_categoryData)
                .CreateProcedure(new SchemaProcedure("SelectCategoryDatumBycategoryId", StoredProcedureType.SelectByKey)
                    .ForTable(_categoryData)
                    .WithParameter(new SchemaColumn() {Name = "CategoryId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("SelectCategoryDatumPaged", StoredProcedureType.SelectPaged)
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
            
            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _categoryData.Name,
                Columns = new string[]
                {
                    "CategoryId",
                    "[Key]"
                }
            });
            
        }

        void CategoryRoles(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_categoryRoles);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_categoryRoles)

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectCategoryRoleById",
                            @" SELECT cr.*, r.[Name] AS RoleName
                                FROM {prefix}_CategoryRoles cr WITH (nolock) 
                                    INNER JOIN {prefix}_Roles r ON cr.RoleId = r.Id                                    
                                WHERE (
                                   cr.Id = @Id
                                )")
                        .ForTable(_categoryRoles)
                        .WithParameter(_categoryRoles.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectCategoryRolesByCategoryId",
                            @" SELECT cr.*, r.[Name] AS RoleName
                                FROM {prefix}_CategoryRoles cr WITH (nolock) 
                                    INNER JOIN {prefix}_Roles r ON cr.RoleId = r.Id                                    
                                WHERE (
                                   cr.CategoryId = @CategoryId
                                )")
                        .ForTable(_categoryRoles)
                        .WithParameter(new SchemaColumn() {Name = "CategoryId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteCategoryRolesByCategoryId", StoredProcedureType.DeleteByKey)
                    .ForTable(_categoryRoles)
                    .WithParameter(new SchemaColumn() {Name = "CategoryId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteCategoryRolesByRoleIdAndCategoryId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_categoryRoles)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "RoleId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "CategoryId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectCategoryRolesPaged", StoredProcedureType.SelectPaged)
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

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _categoryRoles.Name,
                Columns = new string[]
                {
                    "CategoryId",
                    "RoleId"
                }
            });

        }

        void EntityCategories(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_entityCategories);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityCategories)

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityCategoryById",
                            @" SELECT ec.*, c.[Name] AS CategoryName
                                FROM {prefix}_Categories c WITH (nolock) 
                                    INNER JOIN {prefix}_EntityCategories ec ON ec.CategoryId = c.Id                                    
                                WHERE (
                                   ec.Id = @Id
                                )")
                        .ForTable(_categoryRoles)
                        .WithParameter(_categoryRoles.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityCategoriesByEntityId",
                            @" SELECT ec.*, c.[Name] AS CategoryName
                                FROM {prefix}_Categories c WITH (nolock) 
                                    INNER JOIN {prefix}_EntityCategories ec ON ec.CategoryId = c.Id                                    
                                WHERE (
                                   ec.EntityId = @EntityId
                                )")
                        .ForTable(_entityCategories)
                        .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityCategoryByEntityIdAndCategoryId",
                            @" SELECT ec.*, c.[Name] AS CategoryName
                                FROM {prefix}_Categories c WITH (nolock) 
                                    INNER JOIN {prefix}_EntityCategories ec ON ec.CategoryId = c.Id                                    
                                WHERE (
                                   ec.EntityId = @EntityId AND ec.CategoryId = @CategoryId
                                )")
                        .ForTable(_entityCategories)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "EntityId",
                                DbType = DbType.Int32
                            },
                            new SchemaColumn()
                            {
                                Name = "CategoryId",
                                DbType = DbType.Int32
                            }
                        }))
                        


                .CreateProcedure(
                    new SchemaProcedure("DeleteEntityCategoriesByEntityId", StoredProcedureType.DeleteByKey)
                        .ForTable(_entityCategories)
                        .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteEntityCategoryByEntityIdAndCategoryId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_entityCategories)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "CategoryId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectEntityCategoriesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityCategories)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));
            
            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityCategories.Name,
                Columns = new string[]
                {
                    "EntityId",
                    "CategoryId"
                }
            });

        }
        
        #endregion

    }

}
