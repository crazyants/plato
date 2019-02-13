using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<UserRegistration>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISpamOperatorManager<User> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public RegisterViewProvider(
            ISpamOperatorManager<User> spamOperatorManager,
            IClientIpAddress clientIpAddress,
            IPlatoUserStore<User> platoUserStore)
        {
            _spamOperatorManager = spamOperatorManager;
            _clientIpAddress = clientIpAddress;
            _platoUserStore = platoUserStore;
        }
        
        public override  Task<IViewProviderResult> BuildIndexAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(UserRegistration registration, IUpdateModel updater)
        {
            
            // Execute registered spam operators
            var results = await _spamOperatorManager.OperateAsync(SpamOperations.Registration, new User()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                IpV4Address = _clientIpAddress.GetIpV4Address()
            });

            // IF any operators failed and have custom errors
            // ensure we display the operator error message
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
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserRegistration registration, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(registration, context);
            }
            
            // Execute registered spam operators
            var results = await _spamOperatorManager.OperateAsync(SpamOperations.Registration, new User()
            {
                UserName = registration.UserName,
                Email = registration.Email,
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
                var user = await _platoUserStore.GetByUserNameAsync(registration.UserName);
                if (user != null)
                {
                    user.IsSpam = true;
                    await _platoUserStore.UpdateAsync(user);
                }
            }

            return await BuildIndexAsync(registration, context);
        }

    }

}
