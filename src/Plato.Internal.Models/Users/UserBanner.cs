using System.Data;
using Plato.Internal.Models.Annotations;

namespace Plato.Internal.Models.Users
{
    [TableName("Plato_UserBanner")]
    public class UserBanner : UserImage
    {
        public UserBanner()
        {
        }

        public UserBanner(IDataReader reader)
            : base(reader)
        {
        }
    }
}