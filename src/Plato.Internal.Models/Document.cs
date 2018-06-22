using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models
{
    public class BaseDocument : Serializable, IDocument
    {

        public int Id { get; set; }

        //public string Serialize()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}

    }
}
