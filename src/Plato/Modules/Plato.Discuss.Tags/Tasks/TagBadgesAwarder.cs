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
using Plato.Discuss.Tags.Badges;
using Plato.Internal.Models.Badges;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Badges;
using Plato.Internal.Badges.NotificationTypes;
using Plato.Internal.Notifications.Extensions;

namespace Plato.Discuss.Tags.Tasks
{

    public class TagBadgesAwarder : IBackgroundTaskProvider
    {

        private const string Sql = @"                       
                DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                DECLARE @badgeName nvarchar(255) = '{name}';
                DECLARE @threshold int = {threshold};                  
                DECLARE @userId int;
                DECLARE @reactions int;
                DECLARE @myTable TABLE
                (
                    Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                    UserId int NOT NULL
                );
                DECLARE MSGCURSOR CURSOR FOR SELECT et.CreatedUserId, COUNT(et.Id) AS Tags 
                FROM {prefix}_EntityTags et
                WHERE NOT EXISTS (
                   SELECT Id FROM {prefix}_UserBadges ub 
                   WHERE ub.UserId = et.CreatedUserId AND ub.BadgeName = @badgeName
                 )
                GROUP BY et.CreatedUserId
                ORDER BY Tags DESC

                OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;                    
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    IF (@reactions >= @threshold)
                    BEGIN
                        DECLARE @identity int;
                        EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date, @identity OUTPUT;
                        IF (@identity > 0)
                        BEGIN
                            INSERT INTO @myTable (UserId) VALUES (@userId);                     
                        END
                    END;
                    FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;	                    
                END;
                CLOSE MSGCURSOR;
                DEALLOCATE MSGCURSOR;
                SELECT UserId FROM @myTable;";
        
        public int IntervalInSeconds => 120;

        public IEnumerable<Badge> Badges => new[]
        {
            TagBadges.FirstTag,
            TagBadges.BronzeTag,
            TagBadges.SilverTag,
            TagBadges.GoldTAg
        };
        
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;

        public TagBadgesAwarder(
            ICacheManager cacheManager,
            IDbHelper dbHelper,
            IPlatoUserStore<User> userStore,
            INotificationManager<Badge> notificaitonManager,
            IUserReputationAwarder userReputationAwarder,
            IUserNotificationTypeDefaults userNotificationTypeDefaults)
        {
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _userStore = userStore;
            _notificationManager = notificaitonManager;
            _userReputationAwarder = userReputationAwarder;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {
            
            var bot = await _userStore.GetPlatoBotAsync();
            foreach (var badge in this.Badges)
            {

                // Replacements for SQL script
                var replacements = new Dictionary<string, string>()
                {
                    ["{name}"] = badge.Name,
                    ["{threshold}"] = badge.Threshold.ToString()
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
                        foreach (var user in users.Data)
                        {

                            // ---------------
                            // Award badge reputation 
                            // ---------------

                            var badgeReputation = badge.GetReputation();
                            if (badgeReputation.Points != 0)
                            {
                                await _userReputationAwarder.AwardAsync(badgeReputation, user.Id);
                            }

                            // ---------------
                            // Trigger notifications
                            // ---------------

                            // Email notification
                            if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewBadge))
                            {
                                await _notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
                                {
                                    To = user,
                                    From = bot
                                }, (Badge)badge);
                            }

                            // Web notification
                            if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewBadge))
                            {
                                await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                                {
                                    To = user,
                                    From = bot
                                }, (Badge)badge);
                            }

                        }
                    }

                    _cacheManager.CancelTokens(typeof(UserBadgeStore));

                }

            }
            
        }

    }
    
}
