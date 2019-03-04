using System.Collections.Generic;

namespace Plato.Entities
{
    public class ReportReasons
    {

        public static readonly IDictionary<Reason, string> Reasons = new Dictionary<Reason, string>()
        {
            [Reason.Spam] = "Spam / Advertising",
            [Reason.Profanity] = "Profanity",
            [Reason.Inappropriate] = "Inappropriate Content",
            [Reason.OffTopic] = "Off Topic / Irrelevant",
            [Reason.TermsOfService] = "Terms Of Service Violation",
            [Reason.PrivacyPolicy] = "Privacy Policy Violation",
            [Reason.Signature] = "Inappropriate Signature",
        };

        public enum Reason
        {
            Spam = 1,
            Profanity = 2,
            Inappropriate = 3,
            OffTopic = 4,
            TermsOfService = 5,
            PrivacyPolicy = 6,
            Signature = 7
        }

    }

}
