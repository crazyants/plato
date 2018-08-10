using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Reactions.Models
{
    public class DefaultReactions : List<Reaction>
    {

        public DefaultReactions()
        {
            Add(new Reaction()
            {
                Name = "Thumbs Up",
                Description =  "",
                Emoji = "👍",
                IsPositive = true,
            });

            Add(new Reaction()
            {
                Name = "Thumbs Down",
                Description = "",
                Emoji = "👎",
                IsNegative = true,
            });

            Add(new Reaction()
            {
                Name = "Happy",
                Description = "",
                Emoji = "😀",
                IsPositive = true,
            });

            Add(new Reaction()
            {
                Name = "Hooray",
                Description = "",
                Emoji = "🥡",
                IsPositive = true,
            });

            Add(new Reaction()
            {
                Name = "Confused",
                Description = "",
                Emoji = "😟",
                IsPositive = true,
            });

            Add(new Reaction()
            {
                Name = "Heart",
                Description = "",
                Emoji = "❤️",
                IsPositive = true,
            });

        }

    }
}
