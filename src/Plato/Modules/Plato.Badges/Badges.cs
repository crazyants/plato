using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges
{
    public class Badges : IBadgesProvider<Badge>
    {


        private static Action<AwarderContext> VisitsAwarder()
        {
            return (context) =>
            {

                 var dbContextOptions = context.ServiceProvider.GetRequiredService<IOptions<DbContextOptions>>();

                // select users who don't have this badge 
                // but meet the requirements and award the badge

                var sql = @"
                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                    DECLARE @badgeName nvarchar(255) = '{name}';
                    DECLARE @threshold int = {threshold};                  
                    DECLARE @userId int;
                    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
                    WHERE (u.TotalVisits >= @threshold)
                    AND NOT EXISTS (
		                     SELECT Id FROM {prefix}_UserBadges ub 
		                     WHERE ub.UserId = u.Id AND ub.BadgeName = @badgeName
	                    )
                    ORDER BY u.TotalVisits DESC;
                    
                    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
                    WHILE @@FETCH_STATUS = 0
                    BEGIN

	                    EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
	                    FETCH NEXT FROM MSGCURSOR INTO @userId;
	                    
                    END;

                    CLOSE MSGCURSOR;
                    DEALLOCATE MSGCURSOR;";

                sql = sql.Replace("{name}", context.Badge.Name);
                sql = sql.Replace("{threshold}", context.Badge.Threshold.ToString());
                sql = sql.Replace("{prefix}_", dbContextOptions.Value.TablePrefix);
         
                //using (var scope = serviceProvider.CreateScope())
                //{
                //}

                var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
          
                backgroundTaskManager.Start(async (sender, args) =>
                {
                    var dbContext = context.ServiceProvider.GetRequiredService<IDbContext>();
                    using (var db = dbContext)
                    {
                        await db.ExecuteScalarAsync<int>(
                            CommandType.Text, sql);
                    }

                }, 2000);

            };
        }
        
        public static readonly Badge BronzeVisitor =
            new Badge("BronzeVisitor", "Visitor I", BadgeLevel.Bronze, 1, 0, VisitsAwarder());

        public static readonly Badge SilverVisitor =
            new Badge("SilverVisitor", "Visitor II", BadgeLevel.Silver, 10, 10, VisitsAwarder());

        public static readonly Badge GoldVisitor =
            new Badge("GoldVisitor", "Visitor III", BadgeLevel.Gold, 20, 20, VisitsAwarder());
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                BronzeVisitor,
                SilverVisitor,
                GoldVisitor
            };

        }

        public IEnumerable<DefaultBadges<Badge>> GetDefaultBadges()
        {
            return new[]
            {
                new DefaultBadges<Badge>
                {
                    Feature = "Plato.Badges",
                    Badges = GetBadges()
                }
            };
        }
        

    }

}