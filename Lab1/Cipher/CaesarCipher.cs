using System;
using System.Collections.Generic;
using System.Text;
using Lab1.Models.Alphabets;

namespace Lab1.Cipher;

public static class CaesarCipher
{
    public static string Encrypt(string plaintext, Alphabet alphabet, int shiftAmount)
    {
        string normalizedText = NormalizeText(plaintext, alphabet);
        Func<int, int, int> encryptOperation = (textPos, shift) => (textPos + shift) % alphabet.MaxShift;
        string ciphertext = ShiftText(normalizedText, alphabet, shiftAmount, encryptOperation);
        return FormatInGroups(ciphertext);
    }

    public static string Decrypt(string ciphertext, Alphabet alphabet, int shiftAmount)
    {
        string normalizedText = NormalizeText(ciphertext, alphabet);
        Func<int, int, int> decryptOperation = (textPos, shift) => (textPos - shift + alphabet.MaxShift) % alphabet.MaxShift;
        string plaintext = ShiftText(normalizedText, alphabet, shiftAmount, decryptOperation);
        return FormatInGroups(plaintext);
    }

    public static (string Result, int Shift) Cryptanalyze(string ciphertext, Alphabet alphabet)
    {
        string normalizedCiphertext = NormalizeText(ciphertext, alphabet);
        double bestDeviationScore = double.MaxValue;
        int bestShift = 0;

        for (int shift = 0; shift < alphabet.MaxShift; shift++)
        {
            Func<int, int, int> decryptOperation = (textPos, s) => (textPos - s + alphabet.MaxShift) % alphabet.MaxShift;
            string candidatePlaintext = ShiftText(normalizedCiphertext, alphabet, shift, decryptOperation);
            double deviationScore = ComputeFrequencySquaredDeviation(candidatePlaintext, alphabet);

            if (deviationScore < bestDeviationScore)
            {
                bestDeviationScore = deviationScore;
                bestShift = shift;
            }
        }

        return (Decrypt(normalizedCiphertext, alphabet, bestShift), bestShift);
    }

    private static string NormalizeText(string input, Alphabet alphabet)
    {
        string lowercased = input.ToLower();

        if (alphabet.CharsToReplace != null && alphabet.CharsToReplace.Count > 0)
        {
            foreach (KeyValuePair<char, char> replacement in alphabet.CharsToReplace)
                lowercased = lowercased.Replace(replacement.Key.ToString(), replacement.Value.ToString());
        }

        StringBuilder filteredBuilder = new StringBuilder(lowercased.Length);
        for (int i = 0; i < lowercased.Length; i++)
        {
            char currentChar = lowercased[i];
            if (currentChar >= alphabet.StartCharIndex && currentChar <= alphabet.EndCharIndex)
                filteredBuilder.Append(currentChar);
        }

        return filteredBuilder.ToString();
    }

    private static string ShiftText(string inputText, Alphabet alphabet, int shiftAmount, Func<int, int, int> shiftOperation)
    {
        shiftAmount = (shiftAmount % alphabet.MaxShift + alphabet.MaxShift) % alphabet.MaxShift;
        StringBuilder resultBuilder = new StringBuilder(inputText.Length);

        foreach (char currentChar in inputText)
        {
            if (currentChar >= alphabet.StartCharIndex && currentChar <= alphabet.EndCharIndex)
            {
                int textPos = currentChar - alphabet.StartCharIndex;
                int newPos = shiftOperation(textPos, shiftAmount) + alphabet.StartCharIndex;
                resultBuilder.Append((char)newPos);
            }
        }

        return resultBuilder.ToString();
    }

    private static double ComputeFrequencySquaredDeviation(string text, Alphabet alphabet)
    {
        Dictionary<char, int> characterCounts = new Dictionary<char, int>();
        int totalCharacters = 0;

        foreach (char currentChar in text)
        {
            if (alphabet.Frequencies.ContainsKey(currentChar))
            {
                if (!characterCounts.ContainsKey(currentChar)) characterCounts[currentChar] = 0;
                characterCounts[currentChar]++;
                totalCharacters++;
            }
        }

        Dictionary<char, double> observedFrequencies = new Dictionary<char, double>();
        foreach (KeyValuePair<char, double> freqPair in alphabet.Frequencies)
        {
            char ch = freqPair.Key;
            observedFrequencies[ch] = characterCounts.ContainsKey(ch) ? (double)characterCounts[ch] / totalCharacters : 0.0;
        }

        double squaredDeviationSum = 0.0;
        foreach (KeyValuePair<char, double> freqPair in alphabet.Frequencies)
        {
            double expectedFreq = freqPair.Value;
            double observedFreq = observedFrequencies[freqPair.Key];
            squaredDeviationSum += (observedFreq - expectedFreq) * (observedFreq - expectedFreq);
        }

        return squaredDeviationSum;
    }

    private static string FormatInGroups(string text)
    {
        StringBuilder groupedBuilder = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            if (i > 0 && i % 5 == 0)
                groupedBuilder.Append(' ');
            groupedBuilder.Append(text[i]);
        }
        return groupedBuilder.ToString();
    }
}