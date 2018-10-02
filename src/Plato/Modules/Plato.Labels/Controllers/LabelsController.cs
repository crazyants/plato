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

    public class Results
    {

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }
        
        public IEnumerable<Result> Data { get; set; }

    }

    public class Result
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public string Alias { get; set; }

        public FriendlyNumber TotalEntities { get; set; }

        public int Rank { get; set; }

    }

    public class FriendlyNumber
    {

        public string Text { get; set; }

        public int Value { get; set; }

    }

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
            
            PagedResults<Result> results = null;
            if (labels != null)
            {
                results = new PagedResults<Result>
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

                    results.Data.Add(new Result()
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

            Results output = null;
            if (results != null)
            {
                var totalPages = results.Total.ToSafeCeilingDivision(size);
                output = new Results()
                {
                    Page = page,
                    PageSize = size,
                    Total = results.Total,
                    TotalPages = totalPages,
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
                        q.Name.StartsWith(keywords).Or();
                        q.Description.StartsWith(keywords).Or();
                    }
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

    }

}