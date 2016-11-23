using System.Data;
using Plato.Models.Annotations;

namespace Plato.Models.Users
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