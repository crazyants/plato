using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Ideas.Categories.Models;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Categories.ViewProviders
{
    public class IdeaCommentViewProvider : BaseViewProvider<IdeaComment>
    {

        private readonly IEntityStore<Idea> _entityStore;
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }


        public IdeaCommentViewProvider(
            IStringLocalizer<IdeaCommentViewProvider> stringLocalizer,
            IEntityStore<Idea> entityStore,
            ICategoryStore<Category> channelStore, 
            IBreadCrumbManager breadCrumbManager)
        {
            _entityStore = entityStore;
            _channelStore = channelStore;
            _breadCrumbManager = breadCrumbManager;

            T = stringLocalizer;
            S = stringLocalizer;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(IdeaComment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(IdeaComment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(IdeaComment viewModel, IViewProviderContext context)
        {


            var topic = await _entityStore.GetByIdAsync(viewModel.EntityId);
            if (topic == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base discuss controller 
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
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Ideas")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary()
                        {
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[topic.Title], t => t
                    .Action("Display", "Home", "Plato.Ideas", new RouteValueDictionary
                    {
                        ["opts.id"] = topic.Id,
                        ["opts.alias"] = topic.Alias,
                    })
                    .LocalNav()
                );

                builder.Add(S["Edit Reply"]);

            });

            return default(IViewProviderResult);

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(IdeaComment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
