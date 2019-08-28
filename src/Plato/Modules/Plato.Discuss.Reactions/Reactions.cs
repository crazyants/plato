using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Discuss.Reactions
{

    public class Reactions : IReactionsProvider<Reaction>
    {

        public static readonly Reaction ThumbUp =
            new Reaction("DiscussThumbUp", "Thumb Up", "👍", 1, Sentiment.Positive);

        public static readonly Reaction ThumbDown  =
            new Reaction("DiscussThumbDown", "Thumb Down", "👎", -1, Sentiment.Negative);

        public static readonly Reaction Smile =
            new Reaction("DiscussSmile", "Smile", "😄", 0, Sentiment.Positive);

        public static readonly Reaction Congratulations =
            new Reaction("DiscussCongrats", "Congratulations", "🎉", 0, Sentiment.Positive);
        
        public static readonly Reaction Confused =
            new Reaction("DiscussConfused", "Confused", "😕", 0, Sentiment.Negative);

        public static readonly Reaction Angry =
            new Reaction("DiscussAngry", "Angry", "😠", 0, Sentiment.Negative);

        public static readonly Reaction Fearful =
            new Reaction("DiscussFearful", "Fearful", "😨", 0, Sentiment.Negative);

        public static readonly Reaction Triumph =
            new Reaction("DiscussTriumph", "Triumph", "😤", 0, Sentiment.Positive);

        public static readonly Reaction Heart =
            new Reaction("DiscussHeart", "Heart", "❤️", 0, Sentiment.Positive);
        
        public IEnumerable<Reaction> GetReactions()
        {
            return new[]
            {
                ThumbUp,
                ThumbDown,
                Smile,
                Congratulations,
                Confused,
                Angry,
                Fearful,
                Triumph,
                Heart
            };
        }

    }

}
