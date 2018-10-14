using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges.Providers
{
    //public class VisitBadges : IBadgesProvider<Badge>
    //{

    //    public static readonly Badge NewMember =
    //        new Badge("NewMember", "New Member", BadgeLevel.Bronze, Awarder());
        
    //    public static readonly Badge BronzeVisitor =
    //        new Badge("BronzeVisitor", "Visitor I", BadgeLevel.Bronze, 10, 10, Awarder());

    //    public static readonly Badge SilverVisitor =
    //        new Badge("SilverVisitor", "Visitor II", BadgeLevel.Silver, 50, 20, Awarder());

    //    public static readonly Badge GoldVisitor =
    //        new Badge("GoldVisitor", "Visitor III", BadgeLevel.Gold, 100, 30, Awarder());
        
    //    public IEnumerable<Badge> GetBadges()
    //    {
    //        return new[]
    //        {
    //            NewMember,
    //            BronzeVisitor,
    //            SilverVisitor,
    //            GoldVisitor
    //        };

    //    }

    //    private static Action<AwarderContext> Awarder()
    //    {

    //        // select users who don't have the badge but meet
    //        // the badge requirements and award the badge

    //        return (context) =>
    //        {

    //            var dbContext = context.ServiceProvider.GetRequiredService<IDbContext>();
    //            var dbContextOptions = context.ServiceProvider.GetRequiredService<IOptions<DbContextOptions>>();
    //            var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();

    //            var sql = @"
    //                DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
    //                DECLARE @badgeName nvarchar(255) = '{name}';
    //                DECLARE @threshold int = {threshold};                  
    //                DECLARE @userId int;
    //                DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
    //                WHERE (u.TotalVisits >= @threshold)
    //                AND NOT EXISTS (
		  //                   SELECT Id FROM {prefix}_UserBadges ub 
		  //                   WHERE ub.UserId = u.Id AND ub.BadgeName = @badgeName
	   //                 )
    //                ORDER BY u.TotalVisits DESC;
                    
    //                OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
    //                WHILE @@FETCH_STATUS = 0
    //                BEGIN
	   //                 EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
	   //                 FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
    //                END;
    //                CLOSE MSGCURSOR;
    //                DEALLOCATE MSGCURSOR;";

    //            sql = sql.Replace("{name}", context.Badge.Name);
    //            sql = sql.Replace("{threshold}", context.Badge.Threshold.ToString());
    //            sql = sql.Replace("{prefix}_", dbContextOptions.Value.TablePrefix);

    //            // Start task to execute awarder SQL every X seconds
    //            backgroundTaskManager.Start(async (sender, args) =>
    //                {
    //                    using (var db = dbContext)
    //                    {
    //                        // It's safe to use regular SQL here as no user input is supplied
    //                        await db.ExecuteScalarAsync<int>(CommandType.Text, sql);
    //                    }
    //                }, 60 * 1000);

    //        };

    //    }
        
    //}

}