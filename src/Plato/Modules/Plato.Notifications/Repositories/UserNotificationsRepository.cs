using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;

namespace Plato.Notifications.Repositories
{

    public class UserNotificationsRepository : IUserNotificationsRepository<UserNotification>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserNotificationsRepository> _logger;

        public UserNotificationsRepository(
            IDbContext dbContext,
            ILogger<UserNotificationsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<UserNotification> InsertUpdateAsync(UserNotification model)
        {
            var id = await InsertUpdateInternal(
                model.Id,
                model.UserId,
                model.NotificationName,
                model.Title,
                model.Message,
                model.Url,
                model.ReadDate,
                model.CreatedUserId,
                model.CreatedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<UserNotification> SelectByIdAsync(int id)
        {

            UserNotification output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserNotificationById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    output = new UserNotification();
                    output.PopulateModel(reader);
                }

            }

            return output;

        }

        public async Task<IPagedResults<UserNotification>> SelectAsync(params object[] inputParams)
        {
            PagedResults<UserNotification> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserNotificationsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<UserNotification>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new UserNotification();
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

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity mention with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserNotificationById", id);
            }

            return success > 0 ? true : false;

        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string notificationName,
            string title,
            string message,
            string url,
            DateTimeOffset? readDate,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserNotification",
                    id,
                    userId,
                    notificationName.ToEmptyIfNull().TrimToSize(255),
                    title.ToEmptyIfNull().TrimToSize(255),
                    message.ToEmptyIfNull(),
                    url.ToEmptyIfNull().TrimToSize(500),
                    readDate,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }
        
        #endregion

    }

}
