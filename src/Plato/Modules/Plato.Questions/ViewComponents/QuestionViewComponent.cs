using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Questions.ViewComponents
{

    public class QuestionViewComponent : ViewComponent
    {

        private readonly IEntityStore<Question> _entityStore;
        private readonly IEntityReplyStore<Answer> _entityReplyStore;

        public QuestionViewComponent(
            IEntityReplyStore<Answer> entityReplyStore,
            IEntityStore<Question> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<EntityViewModel<Question, Answer>> GetViewModel(
            EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            var topic = await _entityStore.GetByIdAsync(options.Id);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }
            
            // Return view model
            return new EntityViewModel<Question, Answer>
            {
                Options = options,
                Entity = topic
        };

        }

    }

}