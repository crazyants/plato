using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Users.Badges.NotificationTypes;
using Plato.Users.Models;

namespace Plato.Users.Badges
{
    public class ProfileBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge ConfirmedMember =
            new Badge("Confirmed", "I'm legit me", "fal fa-check", BadgeLevel.Bronze, 0, 5, EmailConfirmedAwarder());

        public static readonly Badge Autobiographer =
            new Badge("Autobiographer", "Added a profile", "fal fa-user", BadgeLevel.Bronze, 0, 5, AutobiographerAwarder());

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                ConfirmedMember,
                Autobiographer
            };

        }
        
        static Action<IBadgeAwarderContext> EmailConfirmedAwarder()
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

                    }, 240 * 1000);

            };

        }

        static Action<IBadgeAwarderContext> AutobiographerAwarder()
        {
            return (context) =>
            {

                // Services we need
                var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
                var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
                var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();
                var notificationManager = context.ServiceProvider.GetRequiredService<INotificationManager<Badge>>();

                const string sql = @"
                    DECLARE @dirty bit = 0;
                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                    DECLARE @badgeName nvarchar(255) = '{name}';
                    DECLARE @threshold int = {threshold};                  
                    DECLARE @userId int;
                    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 ud.UserId FROM {prefix}_UserData AS ud
                    WHERE (ud.[Key] = '{key}')
                    AND NOT EXISTS (
		                     SELECT Id FROM {prefix}_UserBadges ub 
		                     WHERE ub.UserId = ud.UserId AND ub.BadgeName = @badgeName
	                    )
                    ORDER BY ud.CreatedDate DESC;
                    
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
                    ["{threshold}"] = context.Badge.Threshold.ToString(),
                    ["{key}"] = typeof(UserDetail).ToString()
                };

                // Start task to execute awarder SQL with replacements every X seconds
                backgroundTaskManager.Start(async (sender, args) =>
                {
                    var dirty = await dbHelper.ExecuteScalarAsync<bool>(sql, replacements);
                    if (dirty)
                    {

                        //// Email notifications
                        //if (user.NotificationEnabled(EmailNotifications.NewBadge))
                        //{
                        //    await notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
                        //    {
                        //        To = user,
                        //    }, context.Badge);
                        //}



                        cacheManager.CancelTokens(typeof(UserBadgeStore));
                    }

                }, 60 * 1000);

            };

        }


    }

}
