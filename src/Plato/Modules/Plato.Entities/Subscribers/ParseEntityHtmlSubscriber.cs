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

            // IDefaultHtmlEncoder ensures user supplied content is correctly HTML encoded
            // before being stored to the database for later display, For performance reasons
            // we saves the HTML to display within the database as opposed to parsing the
            // message everytime it's displayed. For this reason we must ensure the saved HTML
            // is correctly HTML encoded as this may be dislpayed via the Html.Raw HtmlHelper

            // If needed modules can override the IDefaultHtmlEncoder implementation
            // to handle additional scenarios such as parsing markdown or HTML.
            // If a module overrides the IDefaultHtmlEncoder implementation it's
            // the responsibility of the overriding module to safely encode the HTML for display
            return Task.FromResult(_defaultHtmlEncoder.Encode(message));

        }

    }

}
