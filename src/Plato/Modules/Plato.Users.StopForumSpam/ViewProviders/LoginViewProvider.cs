using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
    {

        private readonly ISpamOperatorManager<User> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public LoginViewProvider(
            IClientIpAddress clientIpAddress, 
            ISpamOperatorManager<User> spamOperatorManager)
        {
            _clientIpAddress = clientIpAddress;
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserLogin viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(UserLogin userLogin, IUpdateModel updater)
        {
            
            var user = new User()
            {
                UserName = userLogin.UserName,
                IpV4Address = _clientIpAddress.GetIpV4Address()
            };

            // Execute any registered spam operations
            var results = await _spamOperatorManager.OperateAsync(SpamOperations.Login, user);

            // IF any operations failed ensure we display the operation error message
            var valid = true;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.CustomMessage)
                        {
                            updater.ModelState.AddModelError(string.Empty, result.Operation.Message);
                            valid = false;
                        }
                    }
                }
            }

            return valid;

        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }
        
    }

}
