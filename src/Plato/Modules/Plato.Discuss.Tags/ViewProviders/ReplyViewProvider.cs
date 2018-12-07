using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.Tags.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class ReplyViewProvider : BaseViewProvider<Reply>
    {

        private const string TagsHtmlName = "tags";


        private readonly IEntityReplyStore<Reply> _replyStore;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly ITagStore<Tag> _tagStore;

        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<Discuss.ViewProviders.TopicViewProvider> stringLocalize,
            IPostManager<Reply> replyManager,
            IEntityReplyStore<Reply> replyStore,
            IEntityTagStore<EntityTag> entityTagStore, 
            ITagStore<Tag> tagStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _entityTagStore = entityTagStore;
            _tagStore = tagStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }



        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Reply reply, IViewProviderContext updater)
        {

            var tagsJson = "";
            var entityTags = await GetEntityTagsByEntityReplyIdAsync(reply.Id);
            if (entityTags != null)
            {

                var tags = await _tagStore.QueryAsync()
                    .Select<TagQueryParams>(q =>
                    {
                        q.Id.IsIn(entityTags.Select(e => e.TagId).ToArray());
                    })
                    .OrderBy("Name")
                    .ToList();

                List<TagApiResult> tagsToSerialize = null;
                if (tags != null)
                {
                    tagsToSerialize = new List<TagApiResult>();
                    foreach (var tag in tags.Data)
                    {
                        tagsToSerialize.Add(new TagApiResult()
                        {
                            Id = tag.Id,
                            Name = tag.Name
                        });
                    }
                }

                if (tagsToSerialize != null)
                {
                    tagsJson = tagsToSerialize.Serialize();
                }

            }

            var viewModel = new EditTopicTagsViewModel()
            {
                Tags = tagsJson,
                HtmlName = TagsHtmlName
            };

            return Views(
                View<EditTopicTagsViewModel>("Topic.Tags.Edit.Footer", model => viewModel).Zone("content")
                    .Order(int.MaxValue)
            );


        }

        public override Task<bool> ValidateModelAsync(Reply reply, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {


            return await BuildEditAsync(reply, context);

        }


        #region "Private Methods"

        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityReplyIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return null;
            }

            return await _entityTagStore.GetByEntityReplyId(entityId);

        }

        #endregion
        
    }

}
