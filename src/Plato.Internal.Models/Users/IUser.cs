using System;
using System.Collections.Generic;

namespace Plato.Internal.Models.Users
{
    public interface IUser : ISimpleUser
    {

        string Email { get; set; }

        string TimeZone { get; set; }

        bool ObserveDst { get; set; }

        string Culture { get; set; }

        string IpV4Address { get; set; }

        string IpV6Address { get; set; }

        int Visits { get; set; }

        DateTimeOffset? VisitsUpdatedDate { get; set; }

        int MinutesActive { get; set; }

        DateTimeOffset? MinutesActiveUpdatedDate { get; set; }
        
        int Points { get; set; }

        DateTimeOffset? PointsUpdatedDate { get; set; }
        
        int Rank { get; set; }

        DateTimeOffset? RankUpdatedDate { get; set; }

        string Signature { get; set; }

        IEnumerable<string> RoleNames { get; set; }

        string ResetToken { get; set; }

        string ConfirmationToken { get; set; }
    }

}
