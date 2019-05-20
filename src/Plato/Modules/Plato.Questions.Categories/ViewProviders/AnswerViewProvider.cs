using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Questions.Categories.Models;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Categories.ViewProviders
{
    public class AnswerViewProvider : BaseViewProvider<Answer>
    {

        private readonly IEntityStore<Question> _entityStore;
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }


        public AnswerViewProvider(
            IStringLocalizer<AnswerViewProvider> stringLocalizer,
            IEntityStore<Question> entityStore,
            ICategoryStore<Category> channelStore, 
            IBreadCrumbManager breadCrumbManager)
        {
            _entityStore = entityStore;
            _channelStore = channelStore;
            _breadCrumbManager = breadCrumbManager;

            T = stringLocalizer;
            S = stringLocalizer;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Answer viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Answer viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Answer viewModel, IViewProviderContext context)
        {


            var topic = await _entityStore.GetByIdAsync(viewModel.EntityId);
            if (topic == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _channelStore.GetParentsByIdAsync(topic.CategoryId);

            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Questions"], home => home
                    .Action("Index", "Home", "Plato.Questions")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary()
                        {
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[topic.Title], t => t
                    .Action("Display", "Home", "Plato.Questions", new RouteValueDictionary
                    {
                        ["opts.id"] = topic.Id,
                        ["opts.alias"] = topic.Alias,
                    })
                    .LocalNav()
                );

                builder.Add(S["Edit Answer"]);

            });

            return default(IViewProviderResult);

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Answer viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
