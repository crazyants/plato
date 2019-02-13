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
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin userLogin,
            IViewProviderContext context)
        {
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(userLogin, context);
            }

            // Execute registered spam operators
            var results = await _spamOperatorManager.OperateAsync(SpamOperations.Login, new User()
            {
                UserName = userLogin.UserName,
                Email = userLogin.Email,
                IpV4Address = _clientIpAddress.GetIpV4Address()
            });

            var flagAsSpam = false;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.FlagAsSpam)
                        {
                            flagAsSpam = true;
                        }
                    }
                }
            }

            // Do we need to update the IsSpam flag?
            if (flagAsSpam)
            {
                var user = await _platoUserStore.GetByUserNameAsync(userLogin.UserName);
                if (user != null)
                {
                    user.IsSpam = true;
                    await _platoUserStore.UpdateAsync(user);
                }
            }

            return await BuildIndexAsync(userLogin, context);

        }
        
    }

}
