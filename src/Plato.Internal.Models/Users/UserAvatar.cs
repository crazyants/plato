namespace Plato.Internal.Models.Users
{
    public class UserAvatar
    {

        public string Url { get; }

        public UserAvatar(ISimpleUser user)
        {

            // If we have a photo Url use that
            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                Url = user.PhotoUrl;
                return;
            }

            var letter = user.DisplayName != null 
                ? user.DisplayName.ToUpper().Substring(0, 1) 
                : "-";

            // Else fallback to our letter service
            Url = $"/users/letter/{letter}/{user.PhotoColor}";

        }

    }
}
