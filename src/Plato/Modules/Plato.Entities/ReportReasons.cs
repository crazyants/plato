using System.Collections.Generic;

namespace Plato.Entities
{
    public class ReportReasons
    {

        public static readonly IDictionary<Reason, string> Reasons = new Dictionary<Reason, string>()
        {
            [Reason.Spam] = "Spam / Advertising",
            [Reason.Inaccurate] = "Inaccurate / Out Of Date",
            [Reason.Inappropriate] = "Inappropriate Content",
            [Reason.Profanity] = "Profanity",
            [Reason.OffTopic] = "Off Topic / Irrelevant",
            [Reason.TermsOfService] = "Terms Of Use Violation",
            [Reason.PrivacyPolicy] = "Privacy Policy Violation",
            [Reason.Signature] = "Inappropriate Signature",
        };

        public enum Reason
        {
            Spam = 1,
            Inaccurate = 2,
            Inappropriate = 3,
            Profanity = 4,
            OffTopic = 5,
            TermsOfService = 6,
            PrivacyPolicy = 7,
            Signature = 8
        }

    }

}
