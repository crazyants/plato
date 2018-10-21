using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{
    
    public class EntityUserQueryParams
    {
        public enum SortBy
        {
            CreatedDate,
            TotalReplies
        }

        public int EntityId { get; set; }

        public SortBy Sort { get; set; } = SortBy.TotalReplies;

        public OrderBy Order { get; set; } = OrderBy.Desc;

        public int PageSize { get; set; } = int.MaxValue;

    }

    public interface IEntityUsersRepository
    {
        Task<IEnumerable<EntityUser>> GetUniqueUsers(EntityUserQueryParams queryParams);
    }

    public class EntityUsersRepository : IEntityUsersRepository
    {

        private readonly IDbHelper _dbHelper;

        public EntityUsersRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<IEnumerable<EntityUser>> GetUniqueUsers(EntityUserQueryParams queryParams)
        {

            var sql = @"            
                -- temporary table to hold aggregated data
                DECLARE @t TABLE
                (
	                IndexID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	                UserID int,
	                LastReplyId int,
	                TotalReplies int
                )

                -- insert aggregated data into temporary table
                INSERT INTO @t (UserID, LastReplyId, TotalReplies)
	                SELECT 
	                u.Id AS UserId, 
	                MAX(r.Id) AS LastReplyId,
	                COUNT(r.Id) AS TotalReplies
	                FROM {prefix}_EntityReplies r 
	                JOIN {prefix}_Users u ON r.CreatedUserId = u.Id
	                  WHERE (r.EntityId = {entityId}) 	
	                GROUP BY u.Id
	                
                -- select our real data
                SELECT TOP {pageSize}
	                u.Id AS UserId,
	                u.UserName,
	                u.NormalizedUserName,
	                u.DisplayName,
	                u.FirstName,
	                u.LastName,
	                u.Alias,
	                r.Id AS LastReplyId,
	                r.CreatedDate AS LastReplyDate,
	                t.TotalReplies
                FROM @t t 
                INNER JOIN {prefix}_Users AS u ON u.Id = t.UserID 
                INNER JOIN {prefix}_EntityReplies AS r ON r.Id = t.LastReplyId 
                ORDER BY {sort} {order}
            ";

            var replacements = new AttributeDictionary()
            {
                ["{entityId}"] = queryParams.EntityId.ToString(),
                ["{pageSize}"] = queryParams.PageSize.ToString(),
                ["{sort}"] = GetSortColumn(queryParams.Sort),
                ["{order}"] = queryParams.Order.ToString().ToUpper()
            };

            return await _dbHelper.ExecuteReaderAsync<IEnumerable<EntityUser>>(sql, replacements, async reader =>
            {
                List<EntityUser> output = null;
                if (reader.HasRows)
                {
                    output = new List<EntityUser>();
                    while (await reader.ReadAsync())
                    {
                        var participant = new EntityUser();
                        participant.PopulateModel(reader);
                        output.Add(participant);
                    }
                }
                return output;
            });

        }

        string GetSortColumn(EntityUserQueryParams.SortBy sortBy)
        {
            switch (sortBy)
            {
                case EntityUserQueryParams.SortBy.TotalReplies:
                    return "t.TotalReplies";
                case EntityUserQueryParams.SortBy.CreatedDate:
                    return "r.CreatedDate";
            }

            return "r.CreatedDate";
            
        }

    }


}
