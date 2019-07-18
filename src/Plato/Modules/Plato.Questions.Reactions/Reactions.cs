using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Questions.Reactions
{

    public class Reactions : IReactionsProvider<Reaction>
    {

        public static readonly Reaction ThumbUp =
            new Reaction("Thumb Up", "Thumb Up", "👍", 1, Sentiment.Positive);

        public static readonly Reaction ThumbDown  =
            new Reaction("Thumb Down", "Thumb Down", "👎", -1, Sentiment.Negative);

        public static readonly Reaction Smile =
            new Reaction("Smile", "Smile", "😄", 0, Sentiment.Positive);

        public static readonly Reaction Congratulations =
            new Reaction("Congrats", "Congratulations", "🎉", 0, Sentiment.Positive);
        
        public static readonly Reaction Confused =
            new Reaction("Confused", "Confused", "😕", 0, Sentiment.Negative);

        public static readonly Reaction Angry =
            new Reaction("Angry", "Angry", "😠", 0, Sentiment.Negative);

        public static readonly Reaction Fearful =
            new Reaction("Fearful", "Fearful", "😨", 0, Sentiment.Negative);

        public static readonly Reaction Triumph =
            new Reaction("Triumph", "Triumph", "😤", 0, Sentiment.Positive);

        public static readonly Reaction Heart =
            new Reaction("Heart", "Heart", "❤️", 0, Sentiment.Positive);
        
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
