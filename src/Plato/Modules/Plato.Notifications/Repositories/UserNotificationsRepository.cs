using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Notifications.Abstractions;

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
                output = await context.ExecuteReaderAsync<UserNotification>(
                    CommandType.StoredProcedure,
                    "SelectUserNotificationById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new UserNotification();
                            await reader.ReadAsync();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return output;

        }

        public async Task<IPagedResults<UserNotification>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<UserNotification> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync<IPagedResults<UserNotification>>(
                    CommandType.StoredProcedure,
                    "SelectUserNotificationsPaged",
                    async reader =>
                    {
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
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                        }

                        return output;
                    },
                    dbParams);
              
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
                    "DeleteUserNotificationById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }
        
        public async Task<bool> UpdateReadDateAsync(int userId, DateTimeOffset? readDate)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Updating all UserNotifications.ReadDate values to '{readDate.ToString()}' for user with Id: '{userId}'.");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "UpdateUserNotificationsReadDate",
                    new[]
                    {
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("ReadDate", DbType.DateTimeOffset, readDate),
                    });
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
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("NotificationName", DbType.String, 255, notificationName),
                        new DbParam("Title", DbType.String, 255, title),
                        new DbParam("Message", DbType.String, message),
                        new DbParam("Url", DbType.String, 500, url),
                        new DbParam("ReadDate", DbType.DateTimeOffset, readDate),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }
        
        #endregion

    }

}
