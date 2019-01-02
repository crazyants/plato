using Plato.Entities.Models;

namespace Plato.Discuss.Models
{
    public class Topic : Entity
    {
        public bool IsNewTopic { get; set; }

        public int Offset { get; set; }

        public int SelectedOffset { get; set; }

    }
}
