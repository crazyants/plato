using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;
using Plato.WebApi.Controllers;

namespace Plato.Labels.Controllers
{

    public class LabelsController : BaseWebApiController
    {
        
        private readonly ILabelService<LabelBase> _labelService;
        private readonly IContextFacade _contextFacade;
   
        public LabelsController(
            ILabelService<LabelBase> labelService,
            IContextFacade contextFacade)
        {
            _labelService = labelService;
            _contextFacade = contextFacade;
        }
        
        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(LabelIndexOptions opts, PagerOptions pager)
        {

            if (opts == null)
            {
                opts = new LabelIndexOptions();;
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            if (opts.Sort == LabelSortBy.Auto)
            {
                opts.Sort = LabelSortBy.Entities;
                opts.Order = OrderBy.Desc;
            }

            var labels = await _labelService.GetResultsAsync(opts, pager);

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
                        ["opts.labelId"] = label.Id,
                        ["opts.alias"] = label.Alias
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
                    Page = pager.Page,
                    Size = pager.Size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(pager.Size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }
        
    }

}