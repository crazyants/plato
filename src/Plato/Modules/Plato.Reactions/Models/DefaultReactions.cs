using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Reactions.Models
{
    public class DefaultReactions : List<EntityReacttion>
    {

        public DefaultReactions()
        {
            Add(new EntityReacttion()
            {
                Name = "Thumbs Up",
                Description =  "",
                Emoji = "👍",
                IsPositive = true,
            });

            Add(new EntityReacttion()
            {
                Name = "Thumbs Down",
                Description = "",
                Emoji = "👎",
                IsNegative = true,
            });

            Add(new EntityReacttion()
            {
                Name = "Happy",
                Description = "",
                Emoji = "😀",
                IsPositive = true,
            });

            Add(new EntityReacttion()
            {
                Name = "Hooray",
                Description = "",
                Emoji = "🥡",
                IsPositive = true,
            });

            Add(new EntityReacttion()
            {
                Name = "Confused",
                Description = "",
                Emoji = "😟",
                IsPositive = true,
            });

            Add(new EntityReacttion()
            {
                Name = "Heart",
                Description = "",
                Emoji = "❤️",
                IsPositive = true,
            });

        }

    }
}
