using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Reactions.Models
{
    public class Reaction
    {
        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Emoji { get; set; }

        public bool IsPositive { get; set; }

        public bool IsNeutral { get; set; }

        public bool IsNegative { get; set; }

    }
}
