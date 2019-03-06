using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.ViewProviders
{
    public class CommentViewProvider : BaseViewProvider<Comment>
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly ICategoryStore<ArticleCategory> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }


        public CommentViewProvider(
            IStringLocalizer<CommentViewProvider> stringLocalizer,
            IEntityStore<Article> entityStore,
            ICategoryStore<ArticleCategory> channelStore, 
            IBreadCrumbManager breadCrumbManager)
        {
            _entityStore = entityStore;
            _channelStore = channelStore;
            _breadCrumbManager = breadCrumbManager;

            T = stringLocalizer;
            S = stringLocalizer;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Comment viewModel, IViewProviderContext context)
        {
            
            var entity = await _entityStore.GetByIdAsync(viewModel.EntityId);
            if (entity == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base controller 
            IEnumerable<ArticleCategory> parents = null;
            if (entity.CategoryId > 0)
            {
                parents = await _channelStore.GetParentsByIdAsync(entity.CategoryId);

            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], home => home
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[entity.Title], t => t
                    .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary
                    {
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias,
                    })
                    .LocalNav()
                );

                builder.Add(S["Edit Reply"]);

            });

            return default(IViewProviderResult);

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
