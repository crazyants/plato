using Plato.Entities.Models;

namespace Plato.Discuss.Models
{
    public class Reply : EntityReply
    {

        public bool IsNewReply { get; set; }

        public int Offset { get; set; }

        public int SelectedOffset { get; set; }

    }
}
