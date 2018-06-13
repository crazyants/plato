using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Models
{
    public class Document : IDocument
    {

        public int Id { get; set; }

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
