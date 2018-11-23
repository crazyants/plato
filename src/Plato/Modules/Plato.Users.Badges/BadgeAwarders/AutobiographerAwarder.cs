using System;
using System.Collections.Generic;
using System.Linq;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Extensions;
using Plato.Users.Badges.BadgeProviders;
using Plato.Users.Badges.NotificationTypes;
using Plato.Users.Models;

namespace Plato.Users.Badges.BadgeAwarders
{
    public class AutobiographerAwarder : IBadgesAwarderProvider<Badge>
    {

        private readonly IBackgroundTaskManager _backgroundTaskManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificaitonManager;

        public AutobiographerAwarder(
            IBackgroundTaskManager backgroundTaskManager, 
            ICacheManager cacheManager,
            IDbHelper dbHelper, 
            IPlatoUserStore<User> userStore,
            INotificationManager<Badge> notificaitonManager)
        {
            _backgroundTaskManager = backgroundTaskManager;
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _userStore = userStore;
            _notificaitonManager = notificaitonManager;
        }

        public ICommandResult<Badge> Award(IBadgeAwarderContext<Badge> context)
        {
            // Create result
            var result = new CommandResult<Badge>();

            // Ensure correct notification provider
            if (!context.Badge.Name.Equals(ProfileBadges.Autobiographer.Name, StringComparison.Ordinal))
            {
                return result.Failed();
            }
            
            const string sql = @"             
                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                    DECLARE @badgeName nvarchar(255) = '{name}';
                    DECLARE @threshold int = {threshold};                  
                    DECLARE @userId int;
                    DECLARE @myTable TABLE
                    (
                        Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                        UserId int NOT NULL
                    );
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
                        DECLARE @identity int
	                    EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date, @identity OUTPUT;
                        IF (@identity > 0)
                        BEGIN
                            INSERT INTO @myTable (UserId) VALUES (@userId);                     
                        END
	                    FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
                    END;
                    CLOSE MSGCURSOR;
                    DEALLOCATE MSGCURSOR;
                    SELECT UserId FROM @myTable;";

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{name}"] = context.Badge.Name,
                ["{threshold}"] = context.Badge.Threshold.ToString(),
                ["{key}"] = typeof(UserDetail).ToString()
            };

            // Start task to execute awarder SQL with replacements every X seconds
            _backgroundTaskManager.Start(async (sender, args) =>
            {
                var userIds = await _dbHelper.ExecuteReaderAsync<IList<int>>(sql, replacements, async reader =>
                {
                    var users = new List<int>();
                    while (await reader.ReadAsync())
                    {
                        if (reader.ColumnIsNotNull(0))
                        {
                            users.Add(Convert.ToInt32(reader[0]));
                        }
                    }
                    return users;
                });

                if (userIds?.Count > 9)
                {
                    foreach (var userId in userIds)
                    {
                        var user = await _userStore.GetByIdAsync(userId);
                        if (user != null)
                        {
                            if (user.NotificationEnabled(WebNotifications.NewBadge))
                            {
                                await _notificaitonManager.SendAsync(new Notification(WebNotifications.NewBadge)
                                {
                                    To = user,
                                }, (Badge)context.Badge);
                            }
                        }
                    }
                    _cacheManager.CancelTokens(typeof(UserBadgeStore));
                }

            }, 10 * 1000);
            
            return result.Success(context.Badge);

        }

    }

}
