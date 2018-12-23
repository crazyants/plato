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
            "A18AA4",
            "6E85D9",
            "7A6ED9",
            "966ED9",
            "C66ED9",
            "D96EBF",
            "C7597B",
            "59C7AE",
            "ACC759",
            "C7B459",
            "D48C3E",
            "D4503E",
            "3EB3D4",
            "92C6D5",
            "92D5B0",
            "96B98D",
            "ABB98D",
            "A18DB9",
            "E27D0C",
            "8D9FF0",
            "BAC44A",
            "A79652",
            "E6DF6B",
            "76ADCF",
            "AA8BAE",
            "95C2ED"
        };
   
        public string GetColor()
        {
            return Colors[new Random().Next(0, Colors.Length - 1)];
        }

    }


}
