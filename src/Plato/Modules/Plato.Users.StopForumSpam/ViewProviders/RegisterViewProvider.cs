using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.StopForumSpam.Services;


namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<UserRegistration>
    {
        
        private readonly ISpamOperationManager<User> _spamOperationManager;
        private readonly IClientIpAddress _clientIpAddress;

        public RegisterViewProvider(
            ISpamOperationManager<User> spamOperationManager,
            IClientIpAddress clientIpAddress)
        {
            _spamOperationManager = spamOperationManager;
            _clientIpAddress = clientIpAddress;
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

            var user = new User()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                IpV4Address = _clientIpAddress.GetIpV4Address()
            };

            var results = await _spamOperationManager.ExecuteAsync(SpamOperationTypes.Registration, user);

            if (results != null)
            {
                foreach (var result in results)
                {
                    user = result.Response;
                }
            }
          


            return true;

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }

    }

}
