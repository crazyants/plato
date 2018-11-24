using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Extensions;
using Plato.Notifications.Models;
using Plato.Notifications.Services;
using Plato.Users.Badges.BadgeProviders;
using Plato.Users.Badges.NotificationTypes;
using Plato.Users.Models;

namespace Plato.Users.Badges.BadgeAwarders
{
    public class AutobiographerAwarder : IBadgesAwarderProvider<Badge>
    {
        
        public int IntervalInSeconds { get; set; } = 20;

        private const string Sql = @"             
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



        private readonly IBackgroundTaskManager _backgroundTaskManager;

        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificationManager;

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
            _notificationManager = notificaitonManager;
        }


        public async Task<ICommandResult<Badge>> Award(IBadgeAwarderContext<Badge> context)
        {
            // Create result
            var result = new CommandResult<Badge>();

            //// Ensure correct notification provider
            //if (!context.Badge.Name.Equals(ProfileBadges.Autobiographer.Name, StringComparison.Ordinal))
            //{
            //    return result.Failed();
            //}

            //var contextFacade = context.ServiceProvider.GetRequiredService<IContextFacade>();
            //var userNotificationManager =
            //    context.ServiceProvider.GetRequiredService<IUserNotificationsManager<UserNotification>>();
            //var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
            //var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();
            //var userStore = context.ServiceProvider.GetRequiredService<IPlatoUserStore<User>>();
            //var notificationManager = context.ServiceProvider.GetRequiredService<INotificationManager<Badge>>();
            //var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
            
            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{name}"] = ProfileBadges.Autobiographer.Name,
                ["{threshold}"] = ProfileBadges.Autobiographer.Threshold.ToString(),
                ["{key}"] = typeof(UserDetail).ToString()
            };

            // Start task to execute awarder SQL with replacements every X seconds
            //_backgroundTaskManager.Start(async (services, args) =>
            //{

            ////var dbHelper = services.GetRequiredService<IDbHelper>();
            ////var userStore = services.GetRequiredService<IPlatoUserStore<User>>();
            ////var notificationManager = services.GetRequiredService<INotificationManager<Badge>>();

            var userIds = await _dbHelper.ExecuteReaderAsync<IList<int>>(Sql, replacements, async reader =>
            {
                var users = new List<int>();
                while (await reader.ReadAsync())
                {
                    if (reader.ColumnIsNotNull("UserId"))
                    {
                        users.Add(Convert.ToInt32(reader["UserId"]));
                    }
                }
                return users;
            });

            if (userIds?.Count > 0)
            {

                // Get all users awarded the badge
                var users = await _userStore.QueryAsync()
                    .Take(1, userIds.Count)
                    .Select<UserQueryParams>(q => { q.Id.IsIn(userIds.ToArray()); })
                    .OrderBy("LastLoginDate", OrderBy.Desc)
                    .ToList();

                // Send notificaitons
                if (users != null)
                {
                    foreach (var user in users.Data)
                    {
                        if (user.NotificationEnabled(WebNotifications.NewBadge))
                        {
                           
                            await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                            {
                                To = user,
                            }, ProfileBadges.Autobiographer);
                        }
                    }
                }

                _cacheManager.CancelTokens(typeof(UserBadgeStore));
            }

            //}, 20 * 1000);

            return result.Success(ProfileBadges.Autobiographer);

        }

    }

}
