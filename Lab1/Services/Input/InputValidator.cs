using Lab1.Models.Alphabets;
using System.Linq;
using System.Text;

namespace Lab1.Services.Input;

public class InputValidator
{
    private static readonly char[] _validWhitespaceChars = new char[] { ' ', '\n', '\r' };

    public InputValidationResult Validate(string input, Alphabet alphabet)
    {
        if (string.IsNullOrWhiteSpace(input))
            return InputValidationResult.Error("Введите текст для обработки");

        int validChars = input.Count(c => IsValidCharacter(c, alphabet));

        if (validChars == 0)
            return InputValidationResult.Error("Все символы не соответствуют выбранному алфавиту");

        var invalidChars = input
            .Where(c => !IsValidCharacter(c, alphabet))
            .Distinct()
            .ToList();

        if (invalidChars.Count != 0)
        {
            StringBuilder errorMessage = new StringBuilder("Недопустимые символы в строке: ");
            errorMessage.AppendJoin(", ", invalidChars.Select(c => $"'{c}'"));
            return InputValidationResult.Error(errorMessage.ToString());
        }

        return InputValidationResult.Success();
    }

    private bool IsValidCharacter(char c, Alphabet alphabet)
    {
        return (c >= (char)alphabet.StartCharIndex && c <= (char)alphabet.EndCharIndex)
               || alphabet.CharsToReplace.ContainsKey(c)
               || _validWhitespaceChars.Contains(c);
    }
}