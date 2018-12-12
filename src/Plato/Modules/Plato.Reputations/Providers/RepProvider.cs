using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Reputations.Services;
using Plato.Reputations.Models;

namespace Plato.Reputations.Providers
{
    public class RepProvider : IReputationsProvider<Reputation>
    {

        public static readonly Reputation VisitReputation =
            new Reputation("Visit", "Reputation awared for every unique visit", 1);

        public static readonly Reputation NewTopicReputation =
            new Reputation("New Topic", "Reputation awared for posting a new topic.", 1);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                VisitReputation,
                NewTopicReputation
            };
        }

        //private static Action<IReputationAwarderContext> NewTopicAwarder()
        //{
      
        //    return (context) =>
        //    {

        //        // Services we need
        //        var backgroundTaskManager = context.ServiceProvider.GetRequiredService<ISafeTimerFactory>();
        //        var cacheManager = context.ServiceProvider.GetRequiredService<ICacheManager>();
        //        var dbHelper = context.ServiceProvider.GetRequiredService<IDbHelper>();

        //        const string sql = @"
        //            DECLARE @dirty bit = 0;
        //            DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
        //            DECLARE @reputationName nvarchar(255) = '{name}';
        //            DECLARE @points int = {points};                  
        //            DECLARE @userId int;
        //            DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
        //            WHERE (u.TotalVisits >= 1)
        //            AND NOT EXISTS (
		      //               SELECT Id FROM {prefix}_UserReputations ur 
		      //               WHERE ur.UserId = u.Id AND ur.ReputationName = @reputationName
	       //             )
        //            ORDER BY u.TotalVisits DESC;
                    
        //            OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
        //            WHILE @@FETCH_STATUS = 0
        //            BEGIN
	       //             EXEC {prefix}_InsertUpdateUserReputation 0, @reputationName, @userId, @points, @date;
        //                SET @dirty = 1;
	       //             FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
        //            END;
        //            CLOSE MSGCURSOR;
        //            DEALLOCATE MSGCURSOR;
        //            SELECT @dirty;";


        //        // Replacements for SQL script
        //        var replacements = new Dictionary<string, string>()
        //        {
        //            ["{name}"] = context.Reputation.Name,
        //            ["{points}"] = context.Reputation.Points.ToString()
        //        };

        //        //// Start task to execute awarder SQL with replacements every X seconds
        //        //backgroundTaskManager.Start(async (sender, args) =>
        //        //{
        //        //    var dirty = await dbHelper.ExecuteScalarAsync<bool>(sql, replacements);
        //        //    if (dirty)
        //        //    {
        //        //        //cacheManager.CancelTokens(typeof(UserBadgeStore));
        //        //    }

        //        //}, 60 * 1000);

        //    };

        //}


    }
}
