using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{
    
    public interface IEntityParticipantsRepository<T> : IRepository<T> where T : class
    {

    }

    public class EntityParticipantsRepository : IEntityParticipantsRepository<EntityParticipant>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityParticipantsRepository> _logger;

        public EntityParticipantsRepository(
            IDbContext dbContext, 
            ILogger<EntityParticipantsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public async Task<EntityParticipant> InsertUpdateAsync(EntityParticipant participant)
        {
            var id = await InsertUpdateInternalAsync(
                participant.Id,
                participant.EntityId,
                participant.UserId,
                participant.UserName,
                participant.CreatedDate
            );

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<EntityParticipant> SelectByIdAsync(int id)
        {
            EntityParticipant participant = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityParticipantById", id);

                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    participant = new EntityParticipant();;
                    participant.PopulateModel(reader);
                }
            }

            return participant;
        }

        public async Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            PagedResults<TModel> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityParticipantsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<TModel>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new Entity();
                        entity.PopulateModel(reader);
                        output.Data.Add((TModel)Convert.ChangeType(entity, typeof(TModel)));
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

        public async Task<IPagedResults<EntityParticipant>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityParticipant> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityParticipantsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityParticipant>();
                    while (await reader.ReadAsync())
                    {
                        var participant = new EntityParticipant();
                        participant.PopulateModel(reader);
                        output.Data.Add(participant);
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
                _logger.LogInformation($"Deleting entity participant with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityParticipantById", id);
            }

            return success > 0 ? true : false;

        }
        
        async Task<int> InsertUpdateInternalAsync(
            int id,
            int entityId,
            int userId,
            string userName,
            DateTime? createdDate)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityParticipant",
                    id,
                    entityId,
                    userId,
                    userName,
                    createdDate.ToDateIfNull());
            }
        }

    }

}
