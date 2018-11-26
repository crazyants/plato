using System;

namespace Plato.Internal.Text.Abstractions
{

    public interface IKeyGenerator
    {

        string GenerateKey();

        string GenerateKey(Action<KeyGeneratorOptions> configure);
    }

    public class KeyGeneratorOptions
    {

        public int Iterations { get; set; } = 20;

        public int MinLengthPerIteration { get; set; } = 8;

        public int MaxLengthPerIteration { get; set; } = 12;

        public int MaxLength { get; set; } = 200;

        public string UniqueIdentifier { get; set; }

        public char[] SupressCharacters { get; set; } = new[]
        {
            '0', '1', 'I', 'O', 'i', 'o', 'Q', 'q'
        };

    }

}
