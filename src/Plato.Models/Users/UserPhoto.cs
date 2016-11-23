﻿using System.Data;
using Plato.Models.Annotations;

namespace Plato.Models.Users
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