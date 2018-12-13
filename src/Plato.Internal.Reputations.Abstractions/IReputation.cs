using System;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IReputation
    {

        string Name { get; set; }

        string Description { get; set; }

        int Points { get; set; }

        string Category { get; set; }

    }

}
