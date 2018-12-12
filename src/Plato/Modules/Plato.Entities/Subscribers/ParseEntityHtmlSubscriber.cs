using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.Subscribers
{

    public class ParseEntityHtmlSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IShellDescriptor _shellDescriptor;
        private readonly IDefaultHtmlEncoder _defaultHtmlEncoder;

        public ParseEntityHtmlSubscriber(
            IBroker broker, 
            IShellDescriptor shellDescriptor, 
            IDefaultHtmlEncoder defaultHtmlEncoder)
        {

            _broker = broker;
            _shellDescriptor = shellDescriptor;
            _defaultHtmlEncoder = defaultHtmlEncoder;
        }
        
        public void Subscribe()
        {
            // Add a default subscription to parse entity html
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml",
                Order = short.MinValue // our default parser should always runs first
            }, async message => await ParseEntityHtmlAsync(message.What));
            
        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml"
            }, async message => await ParseEntityHtmlAsync(message.What));
            
        }
        
        private Task<string> ParseEntityHtmlAsync(string message)
        {

            // If needed modules can override the IDefaultHtmlEncoder implementation
            // to handle other scenarios such as parsing markdown or HTML.
            // If a module overrides the IDefaultHtmlEncoder implementation it's
            // the responsibility of the overriding module to safely encode the HTML

            return Task.FromResult(_defaultHtmlEncoder.Encode(message));
        }

    }

}
