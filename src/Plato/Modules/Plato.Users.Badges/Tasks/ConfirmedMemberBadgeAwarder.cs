using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Badges.NotificationTypes;
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
using Plato.Internal.Models.Badges;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Badges;

namespace Plato.Users.Badges.Tasks
{

    public class ConfirmedMemberBadgeAwarder : IBackgroundTaskProvider
    {

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

        public int IntervalInSeconds => 30;

        public IBadge Badge => ProfileBadges.ConfirmedMember;

        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;

        public ConfirmedMemberBadgeAwarder(
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

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {
            
            //var cacheManager = args.ServiceProvider.GetRequiredService<ICacheManager>();
            //var dbHelper = args.ServiceProvider.GetRequiredService<IDbHelper>();
            //var userStore = args.ServiceProvider.GetRequiredService<IPlatoUserStore<User>>();
            //var notificationManager = args.ServiceProvider.GetRequiredService<INotificationManager<Badge>>();
            //var userReputationAwarder = args.ServiceProvider.GetRequiredService<IUserReputationAwarder>();


            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{name}"] = Badge.Name,
                ["{threshold}"] = Badge.Threshold.ToString()
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

                // Send notificaitons
                if (users != null)
                {
                    var bot = await _userStore.GetPlatoBotAsync();
                    foreach (var user in users.Data)
                    {

                        // ---------------
                        // Award badge points 
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
                                From = bot
                            }, (Badge)Badge);
                        }

                        // Web notification
                        if (user.NotificationEnabled(WebNotifications.NewBadge))
                        {
                            await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                            {
                                To = user,
                                From = bot
                            }, (Badge) Badge);
                        }

                    }
                }

                _cacheManager.CancelTokens(typeof(UserBadgeStore));
            }

        }

    }

}
