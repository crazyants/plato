using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Discuss.Reactions
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstReactor =
            new Badge("First Reaction", "Added a reaction", "fal fa-thumbs-up", BadgeLevel.Bronze, 1, Awarder());
        
        public static readonly Badge BronzeReactor =
            new Badge("New Reactor", "Added several reactions", "fal fa-smile", BadgeLevel.Bronze, 20, 5, Awarder());

        public static readonly Badge SilverReactor =
            new Badge("Reactor", "Added several reactions", "fal fa-bullhorn", BadgeLevel.Silver, 50, 20, Awarder());

        public static readonly Badge GoldReactor =
            new Badge("Chain Reactor", "Added many reactions", "fal fa-hands-heart", BadgeLevel.Gold, 100, 30, Awarder());
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                FirstReactor,
                BronzeReactor,
                SilverReactor,
                GoldReactor
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
                    DECLARE @reactions int;
                    DECLARE MSGCURSOR CURSOR FOR SELECT er.CreatedUserId, COUNT(er.Id) AS Reactions 
                    FROM {prefix}_EntityReactions er
                    WHERE NOT EXISTS (
		                     SELECT Id FROM {prefix}_UserBadges ub 
		                     WHERE ub.UserId = er.CreatedUserId AND ub.BadgeName = @badgeName
	                    )
                    GROUP BY er.CreatedUserId
                    ORDER BY Reactions DESC
                    
                    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;                    
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        IF (@reactions >= @threshold)
                        BEGIN
	                        EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
                            SET @dirty = 1;
                        END;
	                    FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;	                    
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