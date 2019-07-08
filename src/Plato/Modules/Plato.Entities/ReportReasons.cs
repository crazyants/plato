using System.Collections.Generic;

namespace Plato.Entities
{
    public class ReportReasons
    {

        public static readonly IDictionary<Reason, string> Reasons = new Dictionary<Reason, string>()
        {
            [Reason.Spam] = "Spam / Advertising",
            [Reason.Inaccurate] = "Inaccurate / Out Of Date",
            [Reason.InappropriateContent] = "Inappropriate Content",
            [Reason.InappropriateLink] = "Inappropriate Link",
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
            InappropriateContent = 3,
            InappropriateLink = 4,
            Profanity = 5,
            OffTopic = 6,
            TermsOfService = 7,
            PrivacyPolicy = 8,
            Signature = 9
        }

    }

}
