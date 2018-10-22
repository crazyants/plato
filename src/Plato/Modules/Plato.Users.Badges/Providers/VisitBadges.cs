using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Users.Badges.Providers
{
    public class VisitBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge NewMember =
            new Badge("New Comer", "Hi, I'm new here", "fal fa-at", BadgeLevel.Bronze, Awarder());
        
        public static readonly Badge BronzeVisitor =
            new Badge("Getting into this", "I'm starting to like this", "fal fa-heart", BadgeLevel.Bronze, 5, 10, Awarder());

        public static readonly Badge SilverVisitor =
            new Badge("I'm a regular here", "I can't kep away", "fal fa-splotch", BadgeLevel.Silver, 10, 20, Awarder());

        public static readonly Badge GoldVisitor =
            new Badge("I may be obsessed", "I'm here all the time", "fal fa-rocket", BadgeLevel.Gold, 20, 30, Awarder());
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                NewMember,
                BronzeVisitor,
                SilverVisitor,
                GoldVisitor
            };

        }

      
        private static Action<IBadgeAwarderContext> Awarder()
        {

            // select users who don't have the badge but meet
            // the badge requirements and award the badge

            return (context) =>
            {

                // Services we need
                var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
                var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
                var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();

                const string sql = @"
                    DECLARE @dirty bit = 0;
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
                        SET @dirty = 1;
	                    FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
                    END;
                    CLOSE MSGCURSOR;
                    DEALLOCATE MSGCURSOR;
                    SELECT @dirty;";
                
                // Replacements for SQL script
                var replacements = new Dictionary<string, string>()
                {
                    ["{name}"] = context.Badge.Name,
                    ["{threshold}"] = context.Badge.Threshold.ToString()
                };

                // Start task to execute awarder SQL with replacements every X seconds
                backgroundTaskManager.Start(async (sender, args) =>
                    {
                        var dirty = await dbHelper.ExecuteScalarAsync<bool>(sql, replacements);
                        if (dirty)
                        {
                            cacheManager.CancelTokens(typeof(UserBadgeStore));
                        }

                    }, 60 * 1000);

            };

        }
        
    }

}