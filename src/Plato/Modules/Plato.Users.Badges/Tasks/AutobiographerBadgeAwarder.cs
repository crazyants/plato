using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Extensions;
using Plato.Users.Badges.BadgeProviders;
using Plato.Badges.NotificationTypes;
using Plato.Internal.Reputations.Abstractions;
using Plato.Users.Models;

namespace Plato.Users.Badges.Tasks
{

    public class AutobiographerBadgeAwarder : IBackgroundTaskProvider
    {
        public int IntervalInSeconds => 120;

        public IBadge Badge => ProfileBadges.Autobiographer;

        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;

        public AutobiographerBadgeAwarder(
            ICacheManager cacheManager,
            IDbHelper dbHelper,
            IPlatoUserStore<User> userStore,
            INotificationManager<Badge> notificaitonManager, 
            IUserReputationAwarder userReputationAwarder)
        {
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _userStore = userStore;
            _notificationManager = notificaitonManager;
            _userReputationAwarder = userReputationAwarder;
        }

        public async Task ExecuteAsync()
        {
            
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
                        DECLARE @identity int;
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
                ["{name}"] = Badge.Name,
                ["{threshold}"] = Badge.Threshold.ToString(),
                ["{key}"] = typeof(UserDetail).ToString()
            };

            var userIds = await _dbHelper.ExecuteReaderAsync<IList<int>>(sql, replacements, async reader =>
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

                        // ---------------
                        // Award badge reputation 
                        // ---------------

                        var badgeReputation = Badge.GetReputation();
                        if (badgeReputation.Points != 0)
                        {
                            await _userReputationAwarder.AwardAsync(badgeReputation, user.Id);
                        }

                        // ---------------
                        // Trigger notifications
                        // ---------------
                        
                        // Email notification
                        if (user.NotificationEnabled(EmailNotifications.NewBadge))
                        {
                            await _notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
                            {
                                To = user,
                            }, (Badge)Badge);
                        }

                        // Web notification
                        if (user.NotificationEnabled(WebNotifications.NewBadge))
                        {
                            await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                            {
                                To = user,
                            }, (Badge) Badge);
                        }

                    }
                }

                _cacheManager.CancelTokens(typeof(UserBadgeStore));
            }

        }

    }
   
}
