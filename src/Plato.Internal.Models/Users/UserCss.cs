namespace Plato.Internal.Models.Users
{

    public class UserCss
    {

        public string VerifiedCss { get; }

        public string StaffCss { get; }

        public string SpamCss { get; }

        public string BannedCss { get; }

        public UserCss(ISimpleUser user)
        {

            if (user.IsVerified)
            {
                VerifiedCss = "is-verified";
            }

            if (user.IsStaff)
            {
                StaffCss = "is-staff";
            }

            if (user.IsSpam)
            {
                SpamCss = "is-spam";
            }

            if (user.IsBanned)
            {
                BannedCss = "is-banned";
            }
            
        }

        public override string  ToString()
        {
            
            if (!string.IsNullOrEmpty(StaffCss))
            {
                return StaffCss;
            }

            if (!string.IsNullOrEmpty(VerifiedCss))
            {
                return VerifiedCss;
            }

            if (!string.IsNullOrEmpty(SpamCss))
            {
                return SpamCss;
            }
            
            if (!string.IsNullOrEmpty(BannedCss))
            {
                return BannedCss;
            }
            
            return string.Empty;

        }
        
    }

}
