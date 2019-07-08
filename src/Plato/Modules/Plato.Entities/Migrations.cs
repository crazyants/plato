using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Entities
{

    public class Migrations : BaseMigrationProvider
    {
   
        private readonly ISchemaBuilder _schemaBuilder;

        public Migrations(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;

            AvailableMigrations = new List<PreparedMigration>
            {
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.1",
                    Statements = v_1_0_1()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.2",
                    Statements = v_1_0_2()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.3",
                    Statements = v_1_0_3()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.4",
                    Statements = v_1_0_4()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.5",
                    Statements = v_1_0_5()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.6",
                    Statements = v_1_0_5()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.7",
                    Statements = v_1_0_5()
                }
            };

        }
        
        private ICollection<string> v_1_0_1()
        {

            var entities = new SchemaTable()
            {
                Name = "Entities",
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
                        Name = "CategoryId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Title",
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
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPrivate",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsLocked",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReplies",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalAnswers",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalParticipants",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalFollows",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalStars",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyViews",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReplies",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyAnswers",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReactions",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyFollows",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReports",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyStars",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyRatings",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
            };
            
            var entityReplies = new SchemaTable()
            {
                Name = "EntityReplies",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsAnswer",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.1";
                    });
                
                // -----------------
                // Entities
                // -----------------

                // Add new columns to entities table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "Entities",
                    Columns = new List<SchemaColumn>()
                    {
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
                        }
                    }
                });

                // Drop & recreate InsertUpdateEntity stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateEntity",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(entities));

                // -----------------
                // Entity Replies
                // -----------------

                // Add new columns to entity replies table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "EntityReplies",
                    Columns = new List<SchemaColumn>()
                    {
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
                        }
                    }
                });

                // Drop & recreate InsertUpdateEntityReply stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateEntityReply",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(entityReplies));
                
                // Add builder results to output
                output.AddRange(builder.Statements);
                
            }

            return output;

        }

        private ICollection<string> v_1_0_2()
        {
            
            // New EntityReplyData table
            var entityReplyData = new SchemaTable()
            {
                Name = "EntityReplyData",
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
                        Name = "ReplyId",
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
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };


            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.2";
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    });

                // Build table
                builder.TableBuilder.CreateTable(entityReplyData);

                // Build procedures
                builder.ProcedureBuilder
                    .CreateDefaultProcedures(entityReplyData)

                    .CreateProcedure(new SchemaProcedure("SelectEntityReplyDatumByReplyId", StoredProcedureType.SelectByKey)
                        .ForTable(entityReplyData)
                        .WithParameter(new SchemaColumn() { Name = "ReplyId", DbType = DbType.Int32 }))

                    .CreateProcedure(new SchemaProcedure("SelectEntityReplyDatumPaged", StoredProcedureType.SelectPaged)
                        .ForTable(entityReplyData)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "[Key]",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }));
                
                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> v_1_0_3()
        {

            // Entity replies
            var entityReplies = new SchemaTable()
            {
                Name = "EntityReplies",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsAnswer",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.3";
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    });

                // update SelectEntityReplyById
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure(
                            "SelectEntityReplyById",
                            @"SELECT r.*, 
                                    c.UserName AS CreatedUserName,                                   
                                    c.DisplayName AS CreatedDisplayName,                                 
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor,
                                    c.SignatureHtml AS CreatedSignatureHtml,
                                    m.UserName AS ModifiedUserName,                                    
                                    m.DisplayName AS ModifiedDisplayName,                                
                                    m.Alias AS ModifiedAlias,
                                    m.PhotoUrl AS ModifiedPhotoUrl,
                                    m.PhotoColor AS ModifiedPhotoColor,
                                    m.SignatureHtml AS ModifiedSignatureHtml
                                FROM {prefix}_EntityReplies r WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users c ON r.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON r.ModifiedUserId = m.Id
                                WHERE (
                                   r.Id = @Id
                                );
                                SELECT * FROM {prefix}_EntityReplyData WITH (nolock) 
                                WHERE (
                                   ReplyId = @Id
                                );")
                        .ForTable(entityReplies)
                        .WithParameter(entityReplies.PrimaryKeyColumn));


                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> v_1_0_4()
        {

            // Add TotalWords to Entities and EntityReplies

            // Entities table - used to re-create InsertUpdate stored procedure
            var entities = new SchemaTable()
            {
                Name = "Entities",
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
                        Name = "CategoryId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Title",
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
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPrivate",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsLocked",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReplies",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalAnswers",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalParticipants",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalFollows",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalStars",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalWords",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyViews",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReplies",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyAnswers",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReactions",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyFollows",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReports",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyStars",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyRatings",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
            };

            // EntityReplies - used to re-create InsertUpdate stored procedure
            var entityReplies = new SchemaTable()
            {
                Name = "EntityReplies",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsAnswer",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalWords",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
            
            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.4";
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    });

                // -----------------
                // Entities
                // -----------------

                // Add new TotalWords column to entities table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "Entities",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "TotalWords",
                            DbType = DbType.Int32
                        }
                    }
                });

                // Drop & recreate InsertUpdateEntity stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateEntity",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(entities));

                // -----------------
                // Entity Replies
                // -----------------

                // Add new TotalWords column to entity replies table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "EntityReplies",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "TotalWords",
                            DbType = DbType.Int32
                        }
                    }
                });

                // Drop & recreate InsertUpdateEntityReply stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateEntityReply",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(entityReplies));
                
                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> v_1_0_5()
        {

            // Update LastReplyDate to support nullable dates

            // Entities table - used to re-create InsertUpdate stored procedure
            var entities = new SchemaTable()
            {
                Name = "Entities",
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
                        Name = "CategoryId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Title",
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
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Html",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Abstract",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Urls",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IsHidden",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPrivate",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsPinned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsDeleted",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsLocked",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsClosed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReplies",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalAnswers",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalParticipants",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReactions",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalFollows",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalReports",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalStars",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalRatings",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "SummedRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MeanRating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalLinks",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalImages",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalWords",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyViews",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReplies",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyAnswers",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReactions",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyFollows",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyReports",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyStars",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "DailyRatings",
                        DbType = DbType.Double
                    },
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
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
                        Name = "EditedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EditedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastReplyDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
            };

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.5";
                    });

                // -----------------
                // Entities
                // -----------------

                builder.TableBuilder.AlterTableColumns(new SchemaTable()
                {
                    Name = "Entities",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "LastReplyDate",
                            DbType = DbType.DateTimeOffset,
                            Nullable = true
                        }
                    }
                });

                // Drop & recreate InsertUpdateEntity stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateEntity",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(entities));
                
                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
