using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.ViewProviders
{
    public class HomeIndexViewProvider : BaseViewProvider<HomeIndexViewModel>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityManager<Entity> _entityManager;

        private readonly HttpRequest _request;


        public HomeIndexViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityManager<Entity> entityManager)
        {
            _entityManager = entityManager;
            _request = httpContextAccessor.HttpContext.Request;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndexViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(
                Views(
                    View<HomeIndexViewModel>("Home.Index.Header", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("header"),
                    View<HomeIndexViewModel>("Home.Index.Tools", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("tools"),
                    View<HomeIndexViewModel>("Home.Index.Sidebar", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("sidebar"),
                    View<HomeIndexViewModel>("Home.Index.Content", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("content")
                ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(HomeIndexViewModel viewModel, IUpdateModel updater)
        {

         
            var model = new HomeIndexViewModel();;

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, updater);
            }

            if (updater.ModelState.IsValid)
            {

                var message = string.Empty;
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }

                var entity = new Entity
                {
                    Title = viewModel.NewEntityViewModel.Title?.Trim(),
                    Message = message.Trim()
                };

                var result = await _entityManager.CreateAsync(entity);
                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }


            }

            return await BuildIndexAsync(viewModel, updater);
            
        }

        public void Creating(object sender, EntityManagerEventArgs e)
        {

        }


        public void Created(object sender, EntityManagerEventArgs e)
        {
          
        }
        
    }

}
