using Plato.Entities.Models;

namespace Plato.Articles.Models
{
    public class ArticleComment : EntityReply
    {

        public bool IsNewReply { get; set; }

        public int Offset { get; set; }

        public int SelectedOffset { get; set; }

    }
}
