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
                    Version = "1.0.3",
                    Statements = v_1_0_3()
                }
            };

        }
        
        private ICollection<string> v_1_0_3()
        {

            // Our columns to drop
            ////var columnsToDrop = new SchemaTable()
            ////{
            ////    Name = "Entities",
            ////    Columns = new List<SchemaColumn>()
            ////    {
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyViews",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyReplies",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyAnswers",
            ////            DbType = DbType.Int32
            ////        },
            ////         new SchemaColumn()
            ////        {
            ////            Name = "DailyReactions",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyFollows",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyReports",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyStars",
            ////            DbType = DbType.Int32
            ////        },
            ////        new SchemaColumn()
            ////        {
            ////            Name = "DailyRatings",
            ////            DbType = DbType.Int32
            ////        }
            ////    }
            ////};

            // Updated entities table
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
                        options.Version = "1.0.3";
                        options.DropProceduresBeforeCreate = true;
                    });

                // Indexes
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = entities.Name,
                    Columns = new string[]
                    {
                    "IsHidden",
                    "IsPrivate",
                    "IsSpam",
                    "IsPinned",
                    "IsDeleted"
                    }
                });



                ////// Drop daily columns
                ////builder.TableBuilder.DropTableColumns(columnsToDrop);

                ////// Recreate default stored procedures
                ////builder.ProcedureBuilder
                ////    .CreateDefaultProcedures(entities)

                ////// Overwrite our SelectEntityById created via CreateDefaultProcedures
                ////// above to also return all EntityData within a second result set
                ////.CreateProcedure(
                ////    new SchemaProcedure(
                ////            $"SelectEntityById",
                ////            @"SELECT e.*, f.ModuleId, 0 AS Rank, 0 AS MaxRank,
                ////                    c.UserName AS CreatedUserName,                              
                ////                    c.DisplayName AS CreatedDisplayName,                                  
                ////                    c.Alias AS CreatedAlias,
                ////                    c.PhotoUrl AS CreatedPhotoUrl,
                ////                    c.PhotoColor AS CreatedPhotoColor,
                ////                    c.SignatureHtml AS CreatedSignatureHtml,
                ////                    c.IsVerified AS CreatedIsVerified,
                ////                    c.IsStaff AS CreatedIsStaff,
                ////                    c.IsSpam AS CreatedIsSpam,
                ////                    c.IsBanned AS CreatedIsBanned,
                ////                    m.UserName AS ModifiedUserName,                                 
                ////                    m.DisplayName AS ModifiedDisplayName,                                
                ////                    m.Alias AS ModifiedAlias,
                ////                    m.PhotoUrl AS ModifiedPhotoUrl,
                ////                    m.PhotoColor AS ModifiedPhotoColor,
                ////                    m.SignatureHtml AS ModifiedSignatureHtml,
                ////                    m.IsVerified AS ModifiedIsVerified,
                ////                    m.IsStaff AS ModifiedIsStaff,
                ////                    m.IsSpam AS ModifiedIsSpam,
                ////                    m.IsBanned AS ModifiedIsBanned,
                ////                    l.UserName AS LastReplyUserName,                                
                ////                    l.DisplayName AS LastReplyDisplayName,                                  
                ////                    l.Alias AS LastReplyAlias,
                ////                    l.PhotoUrl AS LastReplyPhotoUrl,
                ////                    l.PhotoColor AS LastReplyPhotoColor,
                ////                    l.SignatureHtml AS LastReplySignatureHtml,
                ////                    l.IsVerified AS LastReplyIsVerified,
                ////                    l.IsStaff AS LastReplyIsStaff,
                ////                    l.IsSpam AS LastReplyIsSpam,
                ////                    l.IsBanned AS LastReplyIsBanned
                ////                FROM {prefix}_Entities e WITH (nolock) 
                ////                    LEFT OUTER JOIN {prefix}_Users c ON e.CreatedUserId = c.Id
                ////                    LEFT OUTER JOIN {prefix}_Users m ON e.ModifiedUserId = m.Id
                ////                    LEFT OUTER JOIN {prefix}_Users l ON e.LastReplyUserId = l.Id
                ////                    INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                ////                WHERE (
                ////                   e.Id = @Id
                ////                );
                ////                SELECT * FROM {prefix}_EntityData WITH (nolock) 
                ////                WHERE (
                ////                   EntityId = @Id
                ////                );")
                ////        .ForTable(entities)
                ////        .WithParameter(entities.PrimaryKeyColumn))
                
                ////    // SelectEntitiesByFeatureId
                ////    .CreateProcedure(new SchemaProcedure("SelectEntitiesByFeatureId",
                ////            @"SELECT 
                ////                        e.Id,
                ////                        e.ParentId,
                ////                        e.FeatureId,
                ////                        e.CategoryId,
                ////                        e.Title,
                ////                        e.Alias,
                ////                        e.Abstract,                                        
                ////                        e.IsHidden,
                ////                        e.IsPrivate,
                ////                        e.IsSpam,
                ////                        e.IsPinned,
                ////                        e.IsDeleted,
                ////                        e.IsLocked,
                ////                        e.IsClosed,
                ////                        e.TotalViews,
                ////                        e.TotalReplies,
                ////                        e.TotalAnswers,
                ////                        e.TotalParticipants,
                ////                        e.TotalReactions,
                ////                        e.TotalFollows,
                ////                        e.TotalReports,
                ////                        e.TotalStars,
                ////                        e.TotalRatings,
                ////                        e.SummedRating,
                ////                        e.MeanRating,
                ////                        e.TotalLinks,
                ////                        e.TotalImages,
                ////                        e.TotalWords,          
                ////                        e.SortOrder,                                        
                ////                        e.CreatedUserId,
                ////                        e.CreatedDate,
                ////                        e.EditedUserId,
                ////                        e.EditedDate,
                ////                        e.ModifiedUserId,
                ////                        e.ModifiedDate,
                ////                        e.LastReplyId,
                ////                        e.LastReplyUserId,
                ////                        e.LastReplyDate,
                ////                        0 AS [Message], -- not returned for performance reasons
                ////                        0 AS Html,
                ////                        0 AS Urls,
                ////                        0 AS IpV4Address,
                ////                        0 AS IpV6Address,
                ////                        0 AS Rank,
                ////                        0 AS MaxRank,
                ////                        f.ModuleId, 
                ////                        c.UserName AS CreatedUserName,                              
                ////                        c.DisplayName AS CreatedDisplayName,                                  
                ////                        c.Alias AS CreatedAlias,
                ////                        c.PhotoUrl AS CreatedPhotoUrl,
                ////                        c.PhotoColor AS CreatedPhotoColor,
                ////                        c.SignatureHtml AS CreatedSignatureHtml,
                ////                        c.IsVerified AS CreatedIsVerified,
                ////                        c.IsStaff AS CreatedIsStaff,
                ////                        c.IsSpam AS CreatedIsSpam,
                ////                        c.IsBanned AS CreatedIsBanned,
                ////                        m.UserName AS ModifiedUserName,                                 
                ////                        m.DisplayName AS ModifiedDisplayName,                                
                ////                        m.Alias AS ModifiedAlias,
                ////                        m.PhotoUrl AS ModifiedPhotoUrl,
                ////                        m.PhotoColor AS ModifiedPhotoColor,
                ////                        m.SignatureHtml AS ModifiedSignatureHtml,
                ////                        m.IsVerified AS ModifiedIsVerified,
                ////                        m.IsStaff AS ModifiedIsStaff,
                ////                        m.IsSpam AS ModifiedIsSpam,
                ////                        m.IsBanned AS ModifiedIsBanned,
                ////                        l.UserName AS LastReplyUserName,                                
                ////                        l.DisplayName AS LastReplyDisplayName,                                  
                ////                        l.Alias AS LastReplyAlias,
                ////                        l.PhotoUrl AS LastReplyPhotoUrl,
                ////                        l.PhotoColor AS LastReplyPhotoColor,
                ////                        l.SignatureHtml AS LastReplySignatureHtml,
                ////                        l.IsVerified AS LastReplyIsVerified,
                ////                        l.IsStaff AS LastReplyIsStaff,
                ////                        l.IsSpam AS LastReplyIsSpam,
                ////                        l.IsBanned AS LastReplyIsBanned
                ////                    FROM {prefix}_Entities e WITH (nolock) 
                ////                        LEFT OUTER JOIN {prefix}_Users c ON e.CreatedUserId = c.Id
                ////                        LEFT OUTER JOIN {prefix}_Users m ON e.ModifiedUserId = m.Id
                ////                        LEFT OUTER JOIN {prefix}_Users l ON e.LastReplyUserId = l.Id
                ////                        INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                ////                    WHERE (
                ////                       e.FeatureId = @FeatureId
                ////                    )")
                ////        .ForTable(entities)
                ////        .WithParameter(new SchemaColumn() { Name = "FeatureId", DbType = DbType.Int32 })

                ////    );
                
                // Add builder results to output
                output.AddRange(builder.Statements);
                
            }

            return output;

        }

        
    }

}
