using System.Data;
using Plato.Internal.Models.Annotations;

namespace Plato.Internal.Models.Users
{
    [TableName("Plato_UserPhoto")]
    public class UserPhoto : UserImage
    {
        public UserPhoto()
        {
        }

        public UserPhoto(IDataReader reader)
            : base(reader)
        {
        }
    }
}