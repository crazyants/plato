using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.Subscribers
{

    public class ParseEntityUrlsSubscriber : IBrokerSubscriber
    {

        private string _baseUrl;

        private readonly IContextFacade _contextFacade;
        private readonly IImageUriExtractor _imageUriExtractor;
        private readonly IAnchorUriExtractor _anchorUriExtractor;
        private readonly IBroker _broker;

        public ParseEntityUrlsSubscriber(
            IImageUriExtractor imageUriExtractor,
            IAnchorUriExtractor anchorUriExtractor,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _imageUriExtractor = imageUriExtractor;
            _anchorUriExtractor = anchorUriExtractor;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityUrls"
            }, async message => await ParseEntityUrls(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityUrls"
            }, async message => await ParseEntityUrls(message.What));
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        async Task<string> ParseEntityUrls(string html)
        {

            _baseUrl = await _contextFacade.GetBaseUrlAsync();
            
            // Build extracted results
            var urls = TryGetUris(html);
            if (urls != null)
            {
                // Serialize results to return
                return await urls.SerializeAsync();
            }

            // No results found, return empty
            return string.Empty;

        }

        EntityUris TryGetUris(string input)
        {

            EntityUris output = null;

            _imageUriExtractor.BaseUrl = _baseUrl;
            _anchorUriExtractor.BaseUrl = _baseUrl;

            var imageUrls = _imageUriExtractor.Extract(input);
            if (imageUrls != null)
            {
                output = new EntityUris();
                foreach (var imageUrl in imageUrls)
                {
                    output.ImageUrls.Add(new EntityUri()
                    {
                        Uri = imageUrl.AbsoluteUri
                    });
                }
            }

            var anchorUrls = _anchorUriExtractor.Extract(input);
            if (anchorUrls != null)
            {
                if (output == null)
                {
                    output = new EntityUris();
                }
                foreach (var anchorUrl in anchorUrls)
                {
                    output.AnchorUrls.Add(new EntityUri()
                    {
                        Uri = anchorUrl.AbsoluteUri
                    });
                }
            }

            return output;

        }
        
        #endregion
        
    }

}
