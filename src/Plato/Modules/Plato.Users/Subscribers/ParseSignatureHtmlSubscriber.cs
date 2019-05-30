using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Users.Subscribers
{

    public class ParseSignatureHtmlSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IDefaultHtmlEncoder _defaultHtmlEncoder;

        public ParseSignatureHtmlSubscriber(
            IBroker broker,
            IDefaultHtmlEncoder defaultHtmlEncoder)
        {
            _broker = broker;
            _defaultHtmlEncoder = defaultHtmlEncoder;
        }

        public void Subscribe()
        {
            // Add a default subscription to parse signature html
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseSignatureHtml",
                Order = short.MinValue // our default parser should always runs first
            }, async message => await ParseSignatureHtmlAsync(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseSignatureHtml"
            }, async message => await ParseSignatureHtmlAsync(message.What));

        }

        private Task<string> ParseSignatureHtmlAsync(string message)
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
