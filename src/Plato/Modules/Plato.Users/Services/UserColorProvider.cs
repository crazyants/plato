using System;

namespace Plato.Users.Services
{
    
    public class UserColorProvider : IUserColorProvider
    {

        public static string[] Colors = new string[]
        {
            "6AC494",
            "75ADA8",
            "E34B49",
            "6AA7A1",
            "AFC800",
            "8A8FA4",
            "D5E09E",
            "25A681",
            "7DABB0",
            "71A4BC",
            "00F19A",
            "5BBC36",
            "ED3857",
            "FF817F",
            "FEDC7F",
            "BC8971",
            "71BC99",
            "D1AC5B",
            "7397BE",
            "A18AA4"
        };
   
        public string GetColor()
        {
            return Colors[new Random().Next(0, Colors.Length - 1)];
        }

    }


}
