using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Categories.ViewComponents
{

    public class CategoryInputViewComponent : ViewComponent
    {
        private readonly ICategoryStore<CategoryBase> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public CategoryInputViewComponent(
            ICategoryStore<CategoryBase> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryInputOptions options)
        {

            if (options.SelectedCategories == null)
            {
                options.SelectedCategories = new int[0];
            }
            
            var categories = await BuildChannelSelectionsAsync(options);
            var model = new CategoryInputViewModel(categories)
            {
                HtmlName = options.HtmlName
            };

            return View(model);
        }

        private async Task<IList<Selection<CategoryBase>>> BuildChannelSelectionsAsync(
            CategoryInputOptions options)
        {

            if (options == null)
            {
                options = new CategoryInputOptions();
            }
     
            if (options.Categories != null)
            {

                var items = await RecurseCategories(options.Categories);
          
                var selections = items.Select(c => new Selection<CategoryBase>
                    {
                        IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                        Value = c
                    })
                    .OrderBy(r => r.Value)
                    .ToList();

                return selections;

            }

            return null;
        }


        Task<IList<CategoryBase>> RecurseCategories(
            IEnumerable<ICategory> input,
            IList<CategoryBase> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<CategoryBase>();
            }

            var categories = input.ToList();
            foreach (var category in categories)
            {
                if (category.ParentId == id)
                {
                    var indent = "-".Repeat(category.Depth);
                    if (!string.IsNullOrEmpty(indent))
                    {
                        indent += " ";
                    }
                    output.Add(new CategoryBase
                    {
                        Id = category.Id,
                        Name = indent + category.Name
                       
                    });
                    RecurseCategories(categories, output, category.Id);
                }
            }

            return Task.FromResult(output);

        }

    }

}
