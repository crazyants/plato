using Plato.Entities.Models;

namespace Plato.Docs.Models
{
    public class DocComment : EntityReply
    {

        public bool IsNewReply { get; set; }

        public int Offset { get; set; }

        public int SelectedOffset { get; set; }

    }
}
