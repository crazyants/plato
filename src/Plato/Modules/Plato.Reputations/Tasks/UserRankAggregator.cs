using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Reputations.Tasks
{


    public class UserRankAggregator : IBackgroundTaskProvider
    {

        // Selects all users who have been awarded reputation within the last 24 hours
        // and recalculate the total sum of all reputation awareded to any found user
        private const string Sql = @"
                    DECLARE @dirty bit = 0;
                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                    DECLARE @yesterday DATETIME = DATEADD(day, -1, @date);                           
                    DECLARE @userId int;

                    DECLARE @temp TABLE
                    (
	                    [Rank] int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	                    UserID int, 
	                    Total int	                
                    );

                    -- Order last 200 users awarded reputation by totalpoints
                    -- Insert these users into our temporary rank table
                    INSERT INTO @temp (UserID, Total) 
	                    SELECT TOP 200 Id, TotalPoints FROM	{prefix}_Users AS u
                        WHERE EXISTS (
		                     SELECT ur.Id FROM {prefix}_UserReputations ur 
		                     WHERE ur.CreatedUserId = u.Id AND ur.CreatedDate > @yesterday 
	                    )	                 
	                    ORDER BY TotalPoints DESC;

                    -- Now we have a rank update rank for last 200 users awarded reputation since yesterday
                    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
                    WHERE EXISTS (
		                     SELECT ur.Id FROM {prefix}_UserReputations ur 
		                     WHERE ur.CreatedUserId = u.Id AND ur.CreatedDate > @yesterday 
	                    )
                    ORDER BY u.TotalPoints DESC;
                    
                    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
                    WHILE @@FETCH_STATUS = 0
                    BEGIN	                   
                        UPDATE {prefix}_Users SET [Rank] = (
                            SELECT [Rank] FROM @temp WHERE UserId = @userId
                        ) WHERE Id = @userId;
                        SET @dirty = 1;
	                    FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
                    END;
                    CLOSE MSGCURSOR;
                    DEALLOCATE MSGCURSOR;
                    SELECT @dirty;";

        public int IntervalInSeconds => 30;
        
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;

        public UserRankAggregator(
            IDbHelper dbHelper, 
            ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
        }

        public async Task ExecuteAsync()
        {

            var dirty = await _dbHelper.ExecuteScalarAsync<bool>(Sql);
            if (dirty)
            {
                _cacheManager.CancelTokens(typeof(PlatoUserStore));
            }

        }

    }
    
}
