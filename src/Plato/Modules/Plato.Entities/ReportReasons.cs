using System.Collections.Generic;

namespace Plato.Entities
{
    public class ReportReasons
    {

        public static readonly IDictionary<Reason, string> Reasons = new Dictionary<Reason, string>()
        {
            [Reason.Spam] = "Spam / Advertising",
            [Reason.Inaccurate] = "Inaccurate / Out Of Date",
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
            Inaccurate = 2,
            Profanity = 3,
            Inappropriate = 4,
            OffTopic = 5,
            TermsOfService = 6,
            PrivacyPolicy = 7,
            Signature = 8
        }

    }

}
