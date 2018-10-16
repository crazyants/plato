using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Users.Badges.Providers
{
    public class ProfileBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge ConfirmedMember =
            new Badge("Confirmed", "I'm legit me", "fal fa-check", BadgeLevel.Bronze, ConfirmedAwarder());

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                ConfirmedMember
            };

        }
        
        private static Action<IBadgeAwarderContext> ConfirmedAwarder()
        {
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
                    WHERE (u.EmailConfirmed = 1)
                    AND NOT EXISTS (
		                     SELECT Id FROM {prefix}_UserBadges ub 
		                     WHERE ub.UserId = u.Id AND ub.BadgeName = @badgeName
	                    )
                    ORDER BY u.Id DESC;
                    
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
