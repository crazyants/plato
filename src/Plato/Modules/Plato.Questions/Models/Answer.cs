using Plato.Entities.Models;

namespace Plato.Questions.Models
{
    public class Answer : EntityReply
    {

        public bool IsNewAnswer { get; set; }

        public int RowOffset { get; set; }

        public int Offset { get; set; }

    }
}
