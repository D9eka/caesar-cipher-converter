using Lab1.Services.Message;
using Microsoft.UI.Xaml.Data;
using System;

namespace Lab1.Converters
{
    public class MessageTypeToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageType type)
            {
                return type switch
                {
                    MessageType.Error => "✖",   // крестик U+2716
                    MessageType.Warning => "⚠", // предупреждение U+26A0
                    _ => string.Empty
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
