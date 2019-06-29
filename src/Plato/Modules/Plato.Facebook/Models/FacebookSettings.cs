using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Facebook.Models
{
    public class FacebookSettings : Serializable
    {

        public string AppId { get; set; }

        public string AppSecret { get; set; }

    }
}
