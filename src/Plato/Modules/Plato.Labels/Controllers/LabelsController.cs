using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Labels.Controllers
{

    public class LabelsController : BaseWebApiController
    {
        
        private readonly ILabelStore<LabelBase> _labelStore;
        private readonly IContextFacade _contextFacade;
   
        public LabelsController(
            ILabelStore<LabelBase> labelStore,
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade)
        {
            _labelStore = labelStore;
            _contextFacade = contextFacade;
        }
        
        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "TotalEntities",
            OrderBy order = OrderBy.Desc)
        {

            var labels = await GetData(
                page,
                size,
                keywords,
                sort,
                order);
            
            PagedResults<LabelApiResult> results = null;
            if (labels != null)
            {
                results = new PagedResults<LabelApiResult>
                {
                    Total = labels.Total
                };
                
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var label in labels.Data)
                {

                    var url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Id"] = label.Id,
                        ["Alias"] = label.Alias
                    });

                    results.Data.Add(new LabelApiResult()
                    {
                        Id = label.Id,
                        Name = label.Name,
                        Description = label.Description,
                        ForeColor = label.ForeColor,
                        BackColor = label.BackColor,
                        Alias = label.Alias,
                        TotalEntities = new FriendlyNumber()
                        {
                            Text = label.TotalEntities.ToPrettyInt(),
                            Value = label.TotalEntities
                        },
                        Rank = 0
                    });
                }
            }

            LabelApiResults output = null;
            if (results != null)
            {
                output = new LabelApiResults()
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
        
        async Task<IPagedResults<LabelBase>> GetData(
            int page,
            int pageSize,
            string keywords,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _labelStore.QueryAsync()
                .Take(page, pageSize)
                .Select<LabelQueryParams>(q =>
                {
                    if (!String.IsNullOrEmpty(keywords))
                    {
                        q.Keywords.StartsWith(keywords);
                    }
                })
                .OrderBy(sortBy, sortOrder)
                //.OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

        }

    }

}