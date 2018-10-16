using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Reputations.Services
{

    public interface IUserReputationAggregator
    {
        void Invoke();
    }

    /// <summary>
    /// Polls the UserReputation tables and agregates total reputation points into Users.TotalPoints
    /// </summary>
    public class UserReputationAggregator : IUserReputationAggregator
    {

        private readonly IBackgroundTaskManager _backgroundTaskManager;
        private readonly ILogger<UserReputationAggregator> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;

        public UserReputationAggregator(
            IDbHelper dbHelper, 
            IBackgroundTaskManager backgroundTaskManager,
            ILogger<UserReputationAggregator> logger, 
            ICacheManager cacheManager)
        {
            _backgroundTaskManager = backgroundTaskManager;
            _cacheManager = cacheManager;
            _dbHelper = dbHelper;
            _logger = logger;
        }

        public void Invoke()
        {

            // Selects all users who have been awarded reputation within the last 24 hours
            // and recalculates the total sum of all reputation awareded to any found user

            const string sql = @"
                    DECLARE @dirty bit = 0;
                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                    DECLARE @yesterday DATETIME = DATEADD(day, -1, @date);                           
                    DECLARE @userId int;
                    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users AS u
                    WHERE EXISTS (
		                     SELECT ur.Id FROM {prefix}_UserReputations ur 
		                     WHERE ur.UserId = u.Id AND ur.CreatedDate > @yesterday 
	                    )
                    ORDER BY u.TotalPoints DESC;
                    
                    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
                    WHILE @@FETCH_STATUS = 0
                    BEGIN	                   
                        UPDATE {prefix}_Users SET TotalPoints = (
                            SELECT SUM(Points) FROM {prefix}_UserReputations WHERE UserId = @userId
                        ) WHERE Id = @userId;
                        SET @dirty = 1;
	                    FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
                    END;
                    CLOSE MSGCURSOR;
                    DEALLOCATE MSGCURSOR;
                    SELECT @dirty;";

            // Start task to execute SQL every X seconds
            _backgroundTaskManager.Start(async (sender, args) =>
                {
                    var dirty = await _dbHelper.ExecuteScalarAsync<bool>(sql);
                    if (dirty)
                    {
                        _cacheManager.CancelTokens(typeof(PlatoUserStore));
                    }

                }, 90 * 1000);

        }

    }

}
