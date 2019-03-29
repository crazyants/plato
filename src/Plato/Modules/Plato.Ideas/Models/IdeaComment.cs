using Plato.Entities.Models;

namespace Plato.Ideas.Models
{
    public class IdeaComment : EntityReply
    {

        public bool IsNewAnswer { get; set; }

        public int RowOffset { get; set; }

        public int Offset { get; set; }

    }
}
