using Lab1.Data.Alphabets;
using System.Collections.Generic;

namespace Lab1.Data
{
    public static class AlphabetFactory
    {
        public static Alphabet CreateRussianAlphabet()
        {
            double[] frequencies = {
                0.0815, 0.0185, 0.0454, 0.0170, 0.0298, 0.0088, 0.0094, 0.0165, 0.0735,
                0.0121, 0.0349, 0.0440, 0.0321, 0.0670, 0.1097, 0.0281, 0.0473, 0.0547,
                0.0262, 0.0262, 0.0026, 0.0097, 0.0048, 0.0144, 0.0072, 0.0036, 0.0190,
                0.0043, 0.0062, 0.0185, 0.0210, 0.0170
            };

            return new Alphabet(
                AlphabetType.Russian,
                "Русский",
                'а',
                'я',
                new Dictionary<char, char> { { 'ё', 'е' } },
                frequencies
            );
        }

        public static Alphabet CreateEnglishAlphabet()
        {
            double[] frequencies = {
                0.08167, 0.01492, 0.02782, 0.04253, 0.12702, 0.02228, 0.02015, 0.06094,
                0.06966, 0.00153, 0.00772, 0.04025, 0.02406, 0.06749, 0.07507, 0.01929,
                0.00095, 0.05987, 0.06327, 0.09056, 0.02758, 0.00978, 0.02360, 0.00150,
                0.01974, 0.00074
            };

            return new Alphabet(
                AlphabetType.English,
                "English",
                'a',
                'z',
                new Dictionary<char, char>(),
                frequencies
            );
        }
    }
}