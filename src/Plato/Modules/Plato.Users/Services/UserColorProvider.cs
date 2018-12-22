using System;
using System.Collections.Generic;

namespace Plato.Users.Services
{
    
    public class UserColorProvider : IUserColorProvider
    {

        private static readonly IList<UserColor> Colors = new List<UserColor>()
        {
            new UserColor("6AC494"),
            new UserColor("75ADA8"),
            new UserColor("E34B49"),
            new UserColor("6AA7A1"),
            new UserColor("AFC800"),
            new UserColor("8A8FA4"),
            new UserColor("D5E09E"),
            new UserColor("25A681"),
            new UserColor("7DABB0"),
            new UserColor("71A4BC"),
            new UserColor("00F19A"),
            new UserColor("5BBC36"),
            new UserColor("ED3857"),
            new UserColor("FF817F"),
            new UserColor("FEDC7F"),
            new UserColor("BC8971"),
            new UserColor("71BC99"),
            new UserColor("D1AC5B"),
            new UserColor("7397BE"),
            new UserColor("A18AA4")
        };

        public UserColor GetColor()
        {
            return Colors[new Random().Next(0, Colors.Count - 1)];
        }

    }


}
