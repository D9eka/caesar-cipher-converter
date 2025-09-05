using Lab1.Data.Alphabets;
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
                    int new_pos = (pos + shift) % size;
                    char new_c = (char)(alphabet.StartCharIndex + new_pos);
                    sb.Append(new_c);
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
            Dictionary<char, int> counts = new Dictionary<char, int>();
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

            Dictionary<char, double> obs = new Dictionary<char, double>();
            foreach (var kv in alphabet.Frequencies)
            {
                char ch = kv.Key;
                obs[ch] = counts.ContainsKey(ch) ? (double)counts[ch] / total : 0.0;
            }

            double sum_sq = 0.0;
            foreach (var kv in alphabet.Frequencies)
            {
                double exp = kv.Value;
                double ob = obs[kv.Key];
                sum_sq += (ob - exp) * (ob - exp);
            }

            return sum_sq;
        }

        public static string Decode(string text, Alphabet alphabet, int shift)
        {
            return Group(Shift(text, alphabet, -shift));
        }

        public static string Encode(string text, Alphabet alphabet, int shift)
        {
            return Group(Shift(text, alphabet, shift));
        }

        public static string Hack(string text, Alphabet alphabet)
        {
            double min_sum = double.MaxValue;
            int best_k = 0;
            int size = alphabet.MaxShift;

            for (int k = 0; k < size; k++)
            {
                string decoded = Shift(text, alphabet, -k);
                double sum_sq = ComputeSumSq(decoded, alphabet);
                if (sum_sq < min_sum)
                {
                    min_sum = sum_sq;
                    best_k = k;
                }
            }

            return Group(Shift(text, alphabet, -best_k));
        }
    }
}