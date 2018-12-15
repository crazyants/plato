using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Tasks.Abstractions;

//namespace Plato.Internal.Reputations.Tasks
//{
    
//    public class UserRankAggregator : IBackgroundTaskProvider
//    {
        
//        private const string Sql = @"
//                    DECLARE @dirty bit = 0;
//                    DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
//                    DECLARE @yesterday DATETIME = DATEADD(day, -1, @date);                           
//                    DECLARE @userId int;

//                    DECLARE @temp TABLE
//                    (
//	                    [Rank] int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
//	                    UserID int, 
//	                    Total int	                
//                    );

//                    -- Order users by points adding to temp table                    
//                    INSERT INTO @temp (UserID, Total) 
//	                    SELECT Id, Points FROM {prefix}_Users ORDER BY Points DESC;

//                    -- Now we have a ranked list update rank for last 200 users
//                    DECLARE MSGCURSOR CURSOR FOR SELECT TOP 200 u.Id FROM {prefix}_Users ORDER BY RankUpdatedDate;
                    
//                    OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId;                    
//                    WHILE @@FETCH_STATUS = 0
//                    BEGIN	                   
//                        UPDATE {prefix}_Users SET
//                            [Rank] = (SELECT [Rank] FROM @temp WHERE UserId = @userId),
//                            RankUpdatedDate = @date
//                        WHERE Id = @userId;
//                        SET @dirty = 1;
//	                    FETCH NEXT FROM MSGCURSOR INTO @userId;	                    
//                    END;
//                    CLOSE MSGCURSOR;
//                    DEALLOCATE MSGCURSOR;
//                    SELECT @dirty;";

//        public int IntervalInSeconds => 30;
        
//        private readonly ICacheManager _cacheManager;
//        private readonly IDbHelper _dbHelper;

//        public UserRankAggregator(
//            IDbHelper dbHelper, 
//            ICacheManager cacheManager)
//        {
//            _cacheManager = cacheManager;
//            _dbHelper = dbHelper;
//        }

//        public async Task ExecuteAsync()
//        {

//            var dirty = await _dbHelper.ExecuteScalarAsync<bool>(Sql);
//            if (dirty)
//            {
//                _cacheManager.CancelTokens(typeof(PlatoUserStore));
//            }

//        }

//    }
    
//}
