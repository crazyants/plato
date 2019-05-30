using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Questions.Badges;
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
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Notifications.Extensions;

namespace Plato.Questions.Tasks
{
    public class AnswerBadgesAwarder : IBackgroundTaskProvider
    {

        private const string Sql = @"                       
                DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                DECLARE @badgeName nvarchar(255) = '{name}';
                DECLARE @threshold int = {threshold};                  
                DECLARE @userId int;
                DECLARE @replies int;
                DECLARE @myTable TABLE
                (
                    Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                    UserId int NOT NULL
                );
                DECLARE MSGCURSOR CURSOR FOR SELECT er.CreatedUserId, COUNT(er.Id) AS Total 
                FROM {prefix}_EntityReplies er  INNER JOIN {prefix}_Entities e ON e.Id = er.EntityId
                WHERE e.FeatureId = {featureId} AND NOT EXISTS (            
                   SELECT Id FROM {prefix}_UserBadges ub 
                   WHERE ub.UserId = er.CreatedUserId AND ub.BadgeName = @badgeName
                 )
                GROUP BY er.CreatedUserId
                ORDER BY Total DESC

                OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId, @replies;                    
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    IF (@replies >= @threshold)
                    BEGIN
                        DECLARE @identity int;
                        EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date, @identity OUTPUT;
                        IF (@identity > 0)
                        BEGIN
                            INSERT INTO @myTable (UserId) VALUES (@userId);                     
                        END
                    END;
                    FETCH NEXT FROM MSGCURSOR INTO @userId, @replies;	                    
                END;
                CLOSE MSGCURSOR;
                DEALLOCATE MSGCURSOR;
                SELECT UserId FROM @myTable;";


        public int IntervalInSeconds => 240;

        public IEnumerable<Badge> Badges => new[]
        {
            AnswerBadges.First,
            AnswerBadges.Bronze,
            AnswerBadges.Silver,
            AnswerBadges.Gold
        };

        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;

        public AnswerBadgesAwarder(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<Badge> notificationManager,
            IUserReputationAwarder userReputationAwarder,
            IPlatoUserStore<User> userStore,
            IFeatureFacade featureFacade,
            ICacheManager cacheManager,
            IDbHelper dbHelper)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _userReputationAwarder = userReputationAwarder;
            _notificationManager = notificationManager;
            _featureFacade = featureFacade;
            _cacheManager = cacheManager;
            _userStore = userStore;
            _dbHelper = dbHelper;
        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions");
            if (feature == null)
            {
                return;
            }

            var bot = await _userStore.GetPlatoBotAsync();
            foreach (var badge in this.Badges)
            {

                // Replacements for SQL script
                var replacements = new Dictionary<string, string>()
                {
                    ["{name}"] = badge.Name,
                    ["{threshold}"] = badge.Threshold.ToString(),
                    ["{featureId}"] = feature.Id.ToString()
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
                            // Award reputation for new badges
                            // ---------------

                            var badgeReputation = badge.GetReputation();
                            if (badgeReputation.Points != 0)
                            {
                                await _userReputationAwarder.AwardAsync(badgeReputation, user.Id, $"{badge.Name} badge awarded");
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
                                }, badge);
                            }

                            // Web notification
                            if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewBadge))
                            {
                                await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                                {
                                    To = user,
                                    From = bot
                                }, badge);
                            }

                        }
                    }

                    _cacheManager.CancelTokens(typeof(UserBadgeStore));

                }

            }

        }

    }

}
