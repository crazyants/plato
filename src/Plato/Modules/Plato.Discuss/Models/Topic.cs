using System;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;
using System.Collections.Generic;

namespace Plato.Discuss.Models
{
    public class Topic  : Entity
    {

        public IDictionary<Type, ISerializable> MetaData { get; set; } = new Dictionary<Type, ISerializable>();

        public TopicDetails Details { get; set; } = new TopicDetails();

    }
}
