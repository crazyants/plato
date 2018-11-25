using System.Collections.Generic;
using Plato.Badges.Models;
using Plato.Badges.Services;

namespace Plato.Discuss.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstReactor =
            new Badge("First Reaction", "Added a reaction", "fal fa-thumbs-up", BadgeLevel.Bronze, 1);
        
        public static readonly Badge BronzeReactor =
            new Badge("New Reactor", "Added several reactions", "fal fa-smile", BadgeLevel.Bronze, 20, 5);

        public static readonly Badge SilverReactor =
            new Badge("Reactor", "Added several reactions", "fal fa-bullhorn", BadgeLevel.Silver, 50, 20);

        public static readonly Badge GoldReactor =
            new Badge("Chain Reactor", "Added many reactions", "fal fa-hands-heart", BadgeLevel.Gold, 100, 30);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                FirstReactor,
                BronzeReactor,
                SilverReactor,
                GoldReactor
            };

        }
        
        //private static Action<IBadgeAwarderContext> Awarder()
        //{

        //    // select users who don't have the badge but meet
        //    // the badge requirements and award the badge

        //    return (context) =>
        //    {

        //        // Services we need
        //        var backgroundTaskManager = context.ServiceProvider.GetRequiredService<IBackgroundTaskManager>();
        //        var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
        //        var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();
        //        //var notificationManager = context.ServiceProvider.GetRequiredService<INotificationManager<Badge>>();
        //        //var userStore = context.ServiceProvider.GetRequiredService<IPlatoUserStore<User>>();

        //        const string sql = @"
        //            DECLARE @dirty bit = 0;
        //            DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
        //            DECLARE @badgeName nvarchar(255) = '{name}';
        //            DECLARE @threshold int = {threshold};                  
        //            DECLARE @userId int;
        //            DECLARE @reactions int;
        //            DECLARE @myTable TABLE
        //            (
	       //             Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	       //             UserId int NOT NULL
        //            );
        //            DECLARE MSGCURSOR CURSOR FOR SELECT er.CreatedUserId, COUNT(er.Id) AS Reactions 
        //            FROM {prefix}_EntityReactions er
        //            WHERE NOT EXISTS (
		      //               SELECT Id FROM {prefix}_UserBadges ub 
		      //               WHERE ub.UserId = er.CreatedUserId AND ub.BadgeName = @badgeName
	       //             )
        //            GROUP BY er.CreatedUserId
        //            ORDER BY Reactions DESC
                    
        //            OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;                    
        //            WHILE @@FETCH_STATUS = 0
        //            BEGIN
        //                IF (@reactions >= @threshold)
        //                BEGIN
	       //                 EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date;
        //                    INSERT INTO @myTable (UserId) VALUES (@userId);                     
        //                END;
	       //             FETCH NEXT FROM MSGCURSOR INTO @userId, @reactions;	                    
        //            END;
        //            CLOSE MSGCURSOR;
        //            DEALLOCATE MSGCURSOR;
        //            SELECT UserId FROM @myTable;";
                
        //        // Replacements for SQL script
        //        var replacements = new Dictionary<string, string>()
        //        {
        //            ["{name}"] = context.Badge.Name,
        //            ["{threshold}"] = context.Badge.Threshold.ToString()
        //        };

        //        // Start task to execute awarder SQL with replacements every X seconds
        //        backgroundTaskManager.Start(async (sender, args) =>
        //            {

        //                // Execute awarder and retutn all effected UserIds
        //                var userIds = await dbHelper.ExecuteReaderAsync<IEnumerable<int>>(sql, replacements, async reader =>
        //                {
        //                    IList<int> users = null;
        //                    if ((reader != null) && (reader.HasRows))
        //                    {
        //                        users = new List<int>();
        //                        while (await reader.ReadAsync())
        //                        {
        //                            if (reader.ColumnIsNotNull("UserId"))
        //                            {
        //                                users.Add(Convert.ToInt32(reader["UserId"]));
        //                            }
        //                        }
        //                    }
        //                    return users;
        //                });

        //                if (userIds != null)
        //                {
        //                    foreach (var userId in userIds)
        //                    {
        //                        //var user = await userStore.GetByIdAsync(userId);
        //                        //if (user != null)
        //                        //{
        //                        //    if (user.NotificationEnabled(WebNotifications.NewBadge))
        //                        //    {
        //                        //        await notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
        //                        //        {
        //                        //            To = user,
        //                        //        }, context.Badge);
        //                        //    }
        //                        //}
        //                    }
        //                    // Email notifications
                           




        //                    cacheManager.CancelTokens(typeof(UserBadgeStore));
        //                }

        //            }, 60 * 1000);

        //    };

        //}
        
    }

}