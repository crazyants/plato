using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISpamOperatorManager<User> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public LoginViewProvider(
            IClientIpAddress clientIpAddress, 
            ISpamOperatorManager<User> spamOperatorManager,
            IPlatoUserStore<User> platoUserStore)
        {
            _clientIpAddress = clientIpAddress;
            _spamOperatorManager = spamOperatorManager;
            _platoUserStore = platoUserStore;
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
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Login, BuildSpamOperatorModel(userLogin));

            // IF any operators failed ensure we display the operator error message
            var valid = true;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.CustomMessage)
                        {
                            updater.ModelState.AddModelError(string.Empty,
                                !string.IsNullOrEmpty(result.Operation.Message)
                                    ? result.Operation.Message
                                    : $"Sorry but we've identified your details have been used by known spammers.");
                            valid = false;
                        }
                    }
                }
            }

            return valid;

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin userLogin, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(userLogin, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Login, BuildSpamOperatorModel(userLogin));
            
            return await BuildIndexAsync(userLogin, context);

        }


        User BuildSpamOperatorModel(UserLogin userLogin)
        {
            return new User()
            {
                UserName = userLogin.UserName,
                Email = userLogin.Email,
                IpV4Address = _clientIpAddress.GetIpV4Address()
            };
        }
    }

}
