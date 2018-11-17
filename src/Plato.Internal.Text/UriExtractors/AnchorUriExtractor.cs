using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text.UriExtractors
{

    public class AnchorUriExtractor : IAnchorUriExtractor
    {
        public string BaseUrl { get; set; }

        public IEnumerable<Uri> Extract(string html)
        {

            // We need an absolute uri for new Uri()
            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new ArgumentNullException($"You must specify a {nameof(BaseUrl)} property before calling the AnchorUriExtractor.Extract method.");
            }
            
            var urls = new List<Uri>();
            var pattern = @"<a[^>]*?href\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (Match m in matches)
            {
                var url = m.Groups[1].Value;
                if (!string.IsNullOrEmpty(url))
                {
                    // Check for absolute uri
                    var noHttp = url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) == -1;
                    var noHttps = url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1;
                    var relativeUrl = noHttp && noHttps;
                    if (relativeUrl)
                    {
                        if (!url.StartsWith("/"))
                        {
                            url = "/" + url;
                        }
                        url = this.BaseUrl + url;
                    }
                }


                try
                {
                    urls.Add(new Uri(url));
                }
                catch
                {
                    // ignored
                }
            }

            return urls;

        }

    }

}
