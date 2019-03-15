using Plato.Tags.Models;

namespace Plato.Discuss.Tags.Models
{

    /// <summary>
    /// A marker class used for discussion tag view providers
    /// </summary>
    public class Tag : TagAdmin
    {
        public Tag() : base()
        {
        }

        public Tag(TagBase tag) : base(tag)
        {
        }

    }

}
