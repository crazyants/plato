using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{
    
    //public class EntityUserQueryParams
    //{
    //    public enum SortBy
    //    {
    //        CreatedDate,
    //        TotalReplies
    //    }

    //    public int EntityId { get; set; }

    //    public SortBy Sort { get; set; } = SortBy.TotalReplies;

    //    public OrderBy Order { get; set; } = OrderBy.Desc;

    //    public int PageSize { get; set; } = int.MaxValue;

    //}

    public interface IEntityUsersRepository
    {
        Task<IPagedResults<EntityUser>> SelectAsync(params object[] inputParams);
    }

    public class EntityUsersRepository : IEntityUsersRepository
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityUsersRepository> _logger;

        private readonly IDbHelper _dbHelper;

        public EntityUsersRepository(
            IDbHelper dbHelper,
            IDbContext dbContext,
            ILogger<EntityUsersRepository> logger)
        {
            _dbHelper = dbHelper;
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<IPagedResults<EntityUser>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityUser> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEmailsPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityUser>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityUser();
                        entity.PopulateModel(reader);
                        output.Data.Add(entity);
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }

                }
            }

            return output;

        }




        ////public async Task<IEnumerable<EntityUser>> SelectAsync()
        ////{

        ////    var sql = @"
        ////        -- temporary table to hold aggregated data
        ////        DECLARE @t TABLE
        ////        (
	       ////         IndexID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	       ////         UserID int,
	       ////         LastReplyId int,
	       ////         TotalReplies int
        ////        )

        ////        -- insert aggregated data into temporary table
        ////        INSERT INTO @t (UserID, LastReplyId, TotalReplies)
	       ////         SELECT 
	       ////         u.Id AS UserId, 
	       ////         MAX(r.Id) AS LastReplyId,
	       ////         COUNT(r.Id) AS TotalReplies
	       ////         FROM {prefix}_EntityReplies r 
	       ////         JOIN {prefix}_Users u ON r.CreatedUserId = u.Id
	       ////           WHERE (r.EntityId = {entityId}) 	
	       ////         GROUP BY u.Id
	                
        ////        -- select our real data
        ////        SELECT TOP {pageSize}
	       ////         u.Id AS UserId,
	       ////         u.UserName,
	       ////         u.NormalizedUserName,
	       ////         u.DisplayName,
	       ////         u.FirstName,
	       ////         u.LastName,
	       ////         u.Alias,
	       ////         r.Id AS LastReplyId,
	       ////         r.CreatedDate AS LastReplyDate,
	       ////         t.TotalReplies
        ////        FROM @t t 
        ////        INNER JOIN {prefix}_Users AS u ON u.Id = t.UserID 
        ////        INNER JOIN {prefix}_EntityReplies AS r ON r.Id = t.LastReplyId 
        ////        ORDER BY {sort} {order}
        ////    ";

        ////    var replacements = new AttributeDictionary()
        ////    {
        ////        ["{entityId}"] = queryParams.EntityId.ToString(),
        ////        ["{pageSize}"] = queryParams.PageSize.ToString(),
        ////        ["{sort}"] = GetSortColumn(queryParams.Sort),
        ////        ["{order}"] = queryParams.Order.ToString().ToUpper()
        ////    };

        ////    return await _dbHelper.ExecuteReaderAsync<IEnumerable<EntityUser>>(sql, replacements, async reader =>
        ////    {
        ////        var output = new List<EntityUser>();
        ////        while (await reader.ReadAsync())
        ////        {
        ////            var participant = new EntityUser();
        ////            participant.PopulateModel(reader);
        ////            output.Add(participant);
        ////        }
        ////        return output;
        ////    });

        ////}

        //string GetSortColumn(EntityUserQueryParams.SortBy sortBy)
        //{
        //    switch (sortBy)
        //    {
        //        case EntityUserQueryParams.SortBy.TotalReplies:
        //            return "t.TotalReplies";
        //        case EntityUserQueryParams.SortBy.CreatedDate:
        //            return "r.CreatedDate";
        //    }

        //    return "r.CreatedDate";
            
        //}

    }


}
