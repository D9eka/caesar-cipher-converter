using Lab1.Models.Alphabets;
using System.Collections.Generic;
using System.Text;

namespace Lab1.Cipher
{
    public static class CaesarCipher
    {
        private static string Shift(string text, Alphabet alphabet, int shift)
        {
            text = text.ToLowerInvariant();
            StringBuilder sb = new StringBuilder();
            int size = alphabet.MaxShift;
            shift = (shift % size + size) % size;

            foreach (char original in text)
            {
                char c = alphabet.CharsToReplace.TryGetValue(original, out char rep) ? rep : original;
                if (c >= (char)alphabet.StartCharIndex && c <= (char)alphabet.EndCharIndex)
                {
                    int pos = c - alphabet.StartCharIndex;
                    int newPos = (pos + shift) % size;
                    char newC = (char)(alphabet.StartCharIndex + newPos);
                    sb.Append(newC);
                }
            }

            return sb.ToString();
        }

        private static string Group(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (i > 0 && i % 5 == 0)
                {
                    sb.Append(' ');
                }
                sb.Append(s[i]);
            }
            return sb.ToString();
        }

        private static double ComputeSumSq(string text, Alphabet alphabet)
        {
            Dictionary<char, int> counts = new();
            int total = 0;

            foreach (char c in text)
            {
                if (alphabet.Frequencies.ContainsKey(c))
                {
                    if (!counts.ContainsKey(c)) counts[c] = 0;
                    counts[c]++;
                    total++;
                }
            }

            Dictionary<char, double> obs = new();
            foreach (var kv in alphabet.Frequencies)
            {
                char ch = kv.Key;
                obs[ch] = counts.ContainsKey(ch) ? (double)counts[ch] / total : 0.0;
            }

            double sumSq = 0.0;
            foreach (var kv in alphabet.Frequencies)
            {
                double exp = kv.Value;
                double ob = obs[kv.Key];
                sumSq += (ob - exp) * (ob - exp);
            }

            return sumSq;
        }

        public static string Decode(string text, Alphabet alphabet, int shift)
        {
            return Group(Shift(text, alphabet, -shift));
        }

        public static string Encode(string text, Alphabet alphabet, int shift)
        {
            return Group(Shift(text, alphabet, shift));
        }

        public record HackResult(string Text, int Shift);

        public static HackResult Hack(string text, Alphabet alphabet)
        {
            double minSum = double.MaxValue;
            int bestShift = 0;
            int size = alphabet.MaxShift;

            for (int k = 0; k < size; k++)
            {
                string decoded = Shift(text, alphabet, -k);
                double sumSq = ComputeSumSq(decoded, alphabet);
                if (sumSq < minSum)
                {
                    minSum = sumSq;
                    bestShift = k;
                }
            }

            return new HackResult(Group(Shift(text, alphabet, -bestShift)), bestShift);
        }
    }
}
