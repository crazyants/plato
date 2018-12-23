using System.Data;
using Plato.Internal.Models.Annotations;

namespace Plato.Internal.Models.Users
{

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