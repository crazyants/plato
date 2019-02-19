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

            // Build user to validate
            var user = new User()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                IpV4Address = _clientIpAddress.GetIpV4Address(),
                IpV6Address = _clientIpAddress.GetIpV6Address()
            };
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Registration, user);

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

        public override async Task<IViewProviderResult> BuildUpdateAsync(UserRegistration registration, IViewProviderContext context)
        {

            // We need a valid view model
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(registration, context);
            }

            // If we reach the BuildUpdateAsync method the user has
            // been validated & created so retrieve full user from database
            var user = await _platoUserStore.GetByUserNameAsync(registration.UserName);

            //  User not found
            if (user == null)
            {
                return await BuildIndexAsync(registration, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Login, user);

            // Return view
            return await BuildIndexAsync(registration, context);

        }
        
    }

}
