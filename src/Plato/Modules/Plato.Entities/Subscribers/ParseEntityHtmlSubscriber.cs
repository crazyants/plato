using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.Subscribers
{

    public class ParseEntityHtmlSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IDefaultHtmlEncoder _defaultHtmlEncoder;

        public ParseEntityHtmlSubscriber(
            IDefaultHtmlEncoder defaultHtmlEncoder,
            IBroker broker)
        {
            _defaultHtmlEncoder = defaultHtmlEncoder;
            _broker = broker;
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

            // IDefaultHtmlEncoder ensures user supplied input is correctly HTML encoded
            // before being stored to the database for later display, For performance reasons
            // we saves the HTML to display within the database as opposed to parsing the
            // message every time it's displayed. For this reason we must ensure the saved HTML
            // is correctly HTML encoded as this may be displayed via the Html.Raw HtmlHelper

            // If needed modules can override the IDefaultHtmlEncoder implementation
            // to handle additional scenarios such as parsing markdown or HTML.
            // If a module overrides the IDefaultHtmlEncoder implementation it's
            // the responsibility of the overriding module to safely encode the HTML for display
            return Task.FromResult(_defaultHtmlEncoder.Encode(message));

        }

    }

}
