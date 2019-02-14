using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.ViewModels;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<User>
    {

    
        private readonly ISpamChecker _spamChecker;
        
        public AdminViewProvider(
            ISpamChecker spamChecker)
        {
            _spamChecker = spamChecker;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

         
            // Get spam result
            var result = await _spamChecker.CheckAsync(user);
           
            // Build view model
            var viewModel = new UserSpamViewModel()
            {
                Id = user.Id,
                IsNewUser = user.Id == 0,
                SpamCheckerResult = result
            };

            // Build view
            return Views(
                View<UserSpamViewModel>("Admin.Edit.StopForumSpam.Sidebar", model => viewModel).Zone("sidebar")
                    .Order(int.MinValue)
            );
            
        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {
            return await BuildEditAsync(user, context);
        }

        #endregion
        
    }

}
