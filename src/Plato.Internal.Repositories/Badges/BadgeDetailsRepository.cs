using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Internal.Repositories.Badges
{

    public class BadgeDetailsRepository : IBadgeDetailsRepository
    {

        private readonly IDbHelper _dbHelper;

        public BadgeDetailsRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        

        public async Task<IEnumerable<BadgeDetails>> SelectAsync()
        {
            return await SelectInternalAsync();
        }

        public async Task<IEnumerable<BadgeDetails>> SelectByUserIdAsync(int userId)
        {
            return await SelectInternalAsync(userId);
        }

        // -----------

        async Task<IEnumerable<BadgeDetails>> SelectInternalAsync(int userId = 0)
        {

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{userId}"] = userId.ToString()
            };

            var sql = userId > 0
                ? @"SELECT 	
                        BadgeName,
                        COUNT(Id) AS AwardedCount,
                        MIN(CreatedDate) AS FirstAwardedDate,
                        MAX(CreatedDate) AS LastAwardedDate
                    FROM
                        {prefix}_UserBadges
                    WHERE
                        UserId = {userId}
                    GROUP BY BadgeName"
                : @"SELECT 	
                        BadgeName,
                        COUNT(Id) AS AwardedCount,
                        MIN(CreatedDate) AS FirstAwardedDate,
                        MAX(CreatedDate) AS LastAwardedDate
                    FROM {prefix}_UserBadges
                    GROUP BY BadgeName";

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new List<BadgeDetails>();
                while (await reader.ReadAsync())
                {
                    var details = new BadgeDetails();
                    details.PopulateModel(reader);
                    output.Add(details);
                }
                return output;
            });

        }

    }

}
