using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Tags;
using Plato.Tags.Models;
using Plato.Tags.Stores;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Tags.Controllers
{
    
    public class TagController : BaseWebApiController
    {

        private readonly ITagStore<TagBase> _entityStore;
        private readonly IContextFacade _contextFacade;

        public TagController(
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            ITagStore<TagBase> entityStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "TotalEntities",
            OrderBy order = OrderBy.Desc)
        {
            
            var tags = await GetTags(
                page,
                size,
                keywords,
                sort,
                order);

            IPagedResults<TagApiResult> results = null;
            if (tags != null)
            {
                results = new PagedResults<TagApiResult>
                {
                    Total = tags.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var tag in tags.Data)
                {
                    
                    var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Tags",
                        ["controller"] = "Home",
                        ["action"] = "Tag",
                        ["opts.id"] = tag.Id,
                        ["opts.alias"] = tag.Alias
                    });

                    results.Data.Add(new TagApiResult()
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Url = url
                    });

                }

            }

            IPagedApiResults<TagApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<TagApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }
        
        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<TagBase>> GetTags(
            int page,
            int pageSize,
            string keywords,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _entityStore.QueryAsync()
                .Take(page, pageSize)
                .Select<TagQueryParams>(q =>
                {

                    if (!String.IsNullOrEmpty(keywords))
                    {
                        q.Keywords.Like(keywords);
                    }

                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

        #endregion

    }

}
