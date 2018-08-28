using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Moderation.Models
{
    public class ModeratorDocument : Serializable, IDocument
    {

        public int Id { get; set; }

        public IEnumerable<Moderator> Moderators { get; set; } = new List<Moderator>();

    }
}
