using Plato.Entities.Models;

namespace Plato.Discuss.Models
{
    public class Topic  : Entity
    {
        private TopicDetails Details { get; set; } = new TopicDetails();

    }
}
