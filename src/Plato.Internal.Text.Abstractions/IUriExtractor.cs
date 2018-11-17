using System;
using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions
{
    public interface IUriExtractor
    {

        string BaseUrl { get; set; }

        IEnumerable<Uri> Extract(string html);

    }

}
