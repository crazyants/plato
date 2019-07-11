using System.Text;

namespace Plato.Internal.Models.Users
{

    public class UserCss
    {

        public string DisplayName { get; set; } 

        public UserCss(ISimpleUser user)
        {
            PopulateDisplayNameCss(user);
        }

        public void PopulateDisplayNameCss(ISimpleUser user)
        {

            var sb = new StringBuilder();

            if (user.IsStaff)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("is-staff");
            }

            if (user.IsVerified)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("is-verified");
            }
            
            if (user.IsBanned)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("is-banned");
            }
            
            if (user.IsSpam)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("is-spam");
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                DisplayName = sb.ToString();
            }
                
        }

    }
}
