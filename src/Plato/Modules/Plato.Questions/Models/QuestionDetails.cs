using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Questions.Models
{

    public class QuestionDetails : Serializable
    {
        public IEnumerable<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();
    }
    
}
