using System;
using System.Collections.Generic;

namespace Plato.Internal.Models.Users
{

    public interface IUser : IUserMetaData<UserData>, ISimpleUser
    {

        string FirstName { get; set; }

        string LastName { get; set; }

        string Email { get; set; }
        
        string Password { get; set; }
        
        DateTimeOffset? PasswordExpiryDate { get; set; }

        DateTimeOffset? PasswordUpdatedDate { get; set; }
        
        string TimeZone { get; set; }

        bool ObserveDst { get; set; }

        string Culture { get; set; }

        string Theme { get; set; }

        string IpV4Address { get; set; }

        string IpV6Address { get; set; }

        string Biography { get; set; }

        string Location { get; set; }

        string Url { get; set; }
        
        int Visits { get; set; }

        DateTimeOffset? VisitsUpdatedDate { get; set; }

        int MinutesActive { get; set; }

        DateTimeOffset? MinutesActiveUpdatedDate { get; set; }
        
        int Reputation { get; set; }

        DateTimeOffset? ReputationUpdatedDate { get; set; }
        
        int Rank { get; set; }

        DateTimeOffset? RankUpdatedDate { get; set; }
        
        int IsSpamUpdatedUserId { get; set; }

        DateTimeOffset? IsSpamUpdatedDate { get; set; }
        
        int IsVerifiedUpdatedUserId { get; set; }

        DateTimeOffset? IsVerifiedUpdatedDate { get; set; }

        int IsStaffUpdatedUserId { get; set; }

        DateTimeOffset? IsStaffUpdatedDate { get; set; }
        
        int IsBannedUpdatedUserId { get; set; }

        DateTimeOffset? IsBannedUpdatedDate { get; set; }
        
        DateTimeOffset? IsBannedExpiryDate { get; set; }
        
        UserType UserType { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }
        
        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }
        
        IEnumerable<string> RoleNames { get; set; }

        string ResetToken { get; set; }

        string ConfirmationToken { get; set; }
    }

    public enum UserType
    {
        None = 0,
        Bot = 1
    }

}
