using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Discuss.Channels.Stores
{
    //public class ChannelStore<TCategory> : CategoryStore<TCategory> where TCategory : class, ICategory
    //{

    //    private readonly ICacheManager _cacheManager;

    //    public ChannelStore(
    //        ICategoryRepository<TCategory> categoryRepository,
    //        ICacheManager cacheManager,
    //        ILogger<CategoryStore<TCategory>> logger,
    //        IDbQueryConfiguration dbQuery,
    //        ICategoryDataStore<CategoryData> categoryDataStore,
    //        ITypedModuleProvider typedModuleProvider) :
    //        base(categoryRepository,
    //            cacheManager,
    //            logger,
    //            dbQuery,
    //            categoryDataStore,
    //            typedModuleProvider)
    //    {
    //        _cacheManager = cacheManager;
    //    }
        
    //    public override async Task<TCategory> CreateAsync(TCategory model)
    //    {

    //        var result = await base.UpdateAsync(model);
    //        if (result != null)
    //        {
    //            _cacheManager.CancelTokens(typeof(CategoryStore<Channel>));
    //        }

    //        return result;

    //    }

    //    public override async Task<TCategory> UpdateAsync(TCategory model)
    //    {

    //        var result = await base.UpdateAsync(model);
    //        if (result != null)
    //        {
    //            _cacheManager.CancelTokens(typeof(CategoryStore<Channel>));
    //        }

    //        return result;


    //    }

    //    public override async Task<bool> DeleteAsync(TCategory model)
    //    {
    //        var success = await base.DeleteAsync(model);
    //        if (success)
    //        {
    //            _cacheManager.CancelTokens(typeof(CategoryStore<Channel>));
    //        }

    //        return success;

    //    }

    //    public override async Task<TCategory> GetByIdAsync(int id)
    //    {
    //        return await base.GetByIdAsync(id);
    //    }

    //    public override IQuery<TCategory> QueryAsync()
    //    {
    //        return base.QueryAsync();
    //    }

    //    public override async Task<IPagedResults<TCategory>> SelectAsync(params object[] args)
    //    {
    //        return await base.SelectAsync(args);
    //    }

    //    public override async Task<IEnumerable<TCategory>> GetByFeatureIdAsync(int featureId)
    //    {
    //        return await base.GetByFeatureIdAsync(featureId);
    //    }

    //}
}
