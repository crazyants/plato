using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories.Users
{
    //public class UserDetailRepository : IUserDetailRepository<UserDetail>
    //{

    //    #region "Constructor"

    //    public UserDetailRepository(
    //        IDbContext dbContext,
    //        ILogger<UserSecretRepository> logger)
    //    {
    //        _dbContext = dbContext;
    //        _logger = logger;
    //    }

    //    #endregion

    //    #region "Private Methods"

    //    private async Task<int> InsertUpdateInternal(
    //        int id,
    //        int userId,
    //        string key,
    //        string value,
    //        DateTime? createdDate,
    //        int createdUserId,
    //        DateTime? modifiedDate,
    //        int modifiedUserId
    //    )
    //    {
    //        var identity = 0;
    //        using (var context = _dbContext)
    //        {
    //            identity = await context.ExecuteScalarAsync<int>(
    //                CommandType.StoredProcedure,
    //                "InsertUpdateUserDetail",
    //                id,
    //                userId,
    //                key.ToEmptyIfNull().TrimToSize(255),
    //                value.ToEmptyIfNull(),
    //                createdDate,
    //                createdUserId,
    //                modifiedDate,
    //                modifiedUserId);
    //        }

    //        return identity;
    //    }

    //    #endregion

    //    #region "Private Variables"

    //    private readonly IDbContext _dbContext;
    //    private ILogger<UserSecretRepository> _logger;

    //    #endregion

    //    #region "Implementation"

    //    public Task<bool> DeleteAsync(int Id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<UserDetail> InsertUpdateAsync(UserDetail detail)
    //    {
    //        var id = await InsertUpdateInternal(
    //            detail.Id,
    //            detail.UserId,
    //            detail.Key,
    //            detail.Value,
    //            detail.CreatedDate,
    //            detail.CreatedUserId,
    //            detail.ModifiedDate,
    //            detail.ModifiedUserId);

    //        if (id > 0)
    //            return await SelectByIdAsync(id);

    //        return null;
    //    }


    //    public async Task<UserDetail> SelectByIdAsync(int Id)
    //    {
    //        UserDetail detail = null;
    //        using (var context = _dbContext)
    //        {
    //            var reader = await context.ExecuteReaderAsync(
    //                CommandType.StoredProcedure,
    //                "plato_sp_SelectUserDetail", Id);

    //            if (reader != null)
    //            {
    //                await reader.ReadAsync();
    //                detail = new UserDetail();
    //                detail.PopulateModel(reader);
    //            }
    //        }

    //        return detail;
    //    }
        
   
    //    public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
    //    {
    //        throw new NotImplementedException();
    //    }


    //    #endregion

    //}
}