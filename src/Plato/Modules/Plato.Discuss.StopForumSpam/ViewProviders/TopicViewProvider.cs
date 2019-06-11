using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam.ViewProviders
{

    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISpamOperatorManager<Topic> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public TopicViewProvider(
            IPlatoUserStore<User> platoUserStore,
            ISpamOperatorManager<Topic> spamOperatorManager,
            IClientIpAddress clientIpAddress)
        {
            _platoUserStore = platoUserStore;
            _spamOperatorManager = spamOperatorManager;
            _clientIpAddress = clientIpAddress;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Topic topic, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            
            // Build user to validate
            var user = await BuildUserAsync(topic);

            // Could not build user details from supplied username
            if (user == null)
            {
                return true;
            }
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Topic, topic);

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
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(topic, context);
            }

            // Build user to validate
            var user = await BuildUserAsync(topic);

            // Could not build user details from supplied username
            if (user == null)
            {
                return await BuildIndexAsync(topic, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Topic, topic);

            return await BuildIndexAsync(topic, context);
        }

        

        async Task<User> BuildUserAsync(Topic topic)
        {

            var user = await _platoUserStore.GetByIdAsync(topic.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // TODO: Update to use real client IP
            // Ensure we check against the IP address being used at the time of the post
            user.IpV4Address = "77.247.181.163"; // _clientIpAddress.GetIpV4Address();
            user.IpV6Address = "77.247.181.163"; // _clientIpAddress.GetIpV6Address();
            return user;

        }

    }

}
