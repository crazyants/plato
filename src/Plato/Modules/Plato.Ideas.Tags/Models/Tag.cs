using Plato.Tags.Models;

namespace Plato.Ideas.Tags.Models
{

    /// <summary>
    /// A marker class used for discussion tag view providers
    /// </summary>
    public class Tag : TagAdmin
    {
        public Tag() : base()
        {
        }

        public Tag(ITag tag) : base(tag)
        {
        }

    }

}
