using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Extensions;
using Plato.Users.Badges.BadgeProviders;
using Plato.Users.Badges.NotificationTypes;
using Plato.Users.Models;

namespace Plato.Users.Badges.BadgeAwarders
{
    public class AutobiographerAwarder : IBadgeAwarderProvider<Badge>
    {

        private readonly IBackgroundTaskManager _backgroundTaskManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        private readonly IPlatoUserStore<User> _userStore;

        public AutobiographerAwarder(
            IBackgroundTaskManager backgroundTaskManager, 
            ICacheManager cacheManager,
            IDbHelper dbHelper, 
            IPlatoUserStore<User> userStore)
        {
            _backgroundTaskManager = backgroundTaskManager;
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _userStore = userStore;
        }

        public Task<ICommandResult<Badge>> AwardAsync(IBadgeAwarderContext<Badge> context)
        {
            // Create result
            var result = new CommandResult<Badge>();

            // Ensure correct notification provider
            if (!context.Badge.Name.Equals(ProfileBadges.Autobiographer.Name, StringComparison.Ordinal))
            {
                return Task.FromResult((ICommandResult<Badge>)result.Failed());
            }
            
            const string sql = @"
                    DECLARE @dirty bit = 0;
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
	                    EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
                        INSERT INTO @myTable (UserId) VALUES (@userId);
                        SET @dirty = 1;
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
                var userIds = await _dbHelper.ExecuteReaderAsync<IEnumerable<int>>(sql, replacements, async reader =>
                {
                    IList<int> users = null;
                    if ((reader != null) && (reader.HasRows))
                    {
                        users = new List<int>();
                        while (await reader.ReadAsync())
                        {
                            if (reader.ColumnIsNotNull("UserId"))
                            {
                                users.Add(Convert.ToInt32(reader["UserId"]));
                            }
                        }
                    }
                    return users;
                });

                if (userIds != null)
                {

                    foreach (var userId in userIds)
                    {
                        var user = await _userStore.GetByIdAsync(userId);
                        if (user != null)
                        {
                            if (user.NotificationEnabled(WebNotifications.NewBadge))
                            {
                                //await notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                                //{
                                //    To = user,
                                //}, (Badge) context.Badge);
                            }
                        }
                    }


                    _cacheManager.CancelTokens(typeof(UserBadgeStore));
                }

            }, 10 * 1000);
            
            return Task.FromResult((ICommandResult<Badge>) result.Success(context.Badge));

        }

    }
}
