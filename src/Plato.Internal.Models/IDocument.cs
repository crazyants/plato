using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models
{
    public interface IDocument : ISerializable
    {

        int Id { get; set; }

    }

}
