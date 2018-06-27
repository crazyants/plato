using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Resources.Abstractions
{
    
    public class Resource
    {

        // <link rel="stylesheet" href="~/css/vendors/bootstrap.css" />
        // <script src="~/js/vendors/bootstrap.js"></script>
        // <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

        public string Url { get; set; }

        public string CdnUrl { get; set; }

        public ResourceType Type { get; set; }

        public ResourceSection Section { get; set; }

    }
    
    public enum ResourceSection
    {
        Header,
        Body,
        Footer
    }

    public enum ResourceType
    {
        JavaScript,
        Css,
        Meta
    }

}
