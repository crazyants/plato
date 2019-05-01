using Plato.Entities.Models;

namespace Plato.Issues.Models
{
    public class Comment : EntityReply
    {

        public bool IsNewReply { get; set; }

        public int RowOffset { get; set; }

        public int Offset { get; set; }

    }
}
