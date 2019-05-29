using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Badges;
using Plato.Internal.Badges.NotificationTypes;
using Plato.Internal.Notifications.Extensions;

namespace Plato.Users.Badges.Tasks
{
 
    public class PersonalizerBadgeAwarder : IBackgroundTaskProvider
    {

        private const string Sql = @"             
            DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
            DECLARE @badgeName nvarchar(255) = '{name}';              
            DECLARE @userId int;
            DECLARE @myTable TABLE
            (
                Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                UserId int NOT NULL
            );
            DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
            WHERE (u.Signature != '')
            AND NOT EXISTS (
		             SELECT Id FROM {prefix}_UserBadges ub 
		             WHERE ub.UserId = u.Id AND ub.BadgeName = @badgeName
	            )
            ORDER BY u.ModifiedDate DESC;
            
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

        public int IntervalInSeconds => 240;

        public IBadge Badge => ProfileBadges.Personalizer;

        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;

        public PersonalizerBadgeAwarder(
            ICacheManager cacheManager,
            IDbHelper dbHelper,
            IPlatoUserStore<User> userStore,
            INotificationManager<Badge> notificationManager,
            IUserReputationAwarder userReputationAwarder,
            IUserNotificationTypeDefaults userNotificationTypeDefaults)
        {
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _userStore = userStore;
            _notificationManager = notificationManager;
            _userReputationAwarder = userReputationAwarder;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{name}"] = Badge.Name
            };

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

                // Send notifications
                if (users != null)
                {
                    var bot = await _userStore.GetPlatoBotAsync();
                    foreach (var user in users.Data)
                    {

                        // ---------------
                        // Award badge reputation 
                        // ---------------

                        var badgeReputation = Badge.GetReputation();
                        if (badgeReputation.Points != 0)
                        {
                            await _userReputationAwarder.AwardAsync(badgeReputation, user.Id, $"{Badge.Name} badge awarded");
                        }

                        // ---------------
                        // Trigger notifications
                        // ---------------

                        // Email notification
                        if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewBadge))
                        {
                            await _notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
                            {
                                To = user
                            }, (Badge)Badge);
                        }

                        // Web notification
                        if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewBadge))
                        {
                            await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                            {
                                To = user,
                                From = bot
                            }, (Badge)Badge);
                        }

                    }
                }

                _cacheManager.CancelTokens(typeof(UserBadgeStore));

            }

        }

    }

}
