using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Abstract;

namespace Plato.Internal.Repositories.Abstract
{
    public class DocumentRepository : IDocumentRepository<DocumentEntry>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<DictionaryRepository> _logger;

        #endregion

        #region "Constructor"

        public DocumentRepository(
            IDbContext dbContext,
            ILogger<DictionaryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentEntry> InsertUpdateAsync(DocumentEntry entity)
        {
            var id = await InsertUpdateInternal(
                entity.Id,
                entity.Value,
                entity.CreatedDate,
                entity.CreatedUserId,
                entity.ModifiedDate,
                entity.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public Task<DocumentEntry> SelectByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId
        )
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting document with value: {value}"
                    : $"Updating document with id: {id}");
            }

            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateDocument",
                    id,
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId);
            }

        }


        #endregion

    }

}
