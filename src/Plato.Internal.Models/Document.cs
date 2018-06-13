using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Plato.Internal.Models
{
    public class Document : IDocument
    {

        public int Id { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
