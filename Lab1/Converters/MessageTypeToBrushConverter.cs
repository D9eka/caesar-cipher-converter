using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Lab1.Converters
{
    public class MessageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Lab1.MessageType type)
            {
                switch (type)
                {
                    case Lab1.MessageType.Error:
                        return new SolidColorBrush(Colors.IndianRed);   // красный
                    case Lab1.MessageType.Warning:
                        return new SolidColorBrush(Colors.Goldenrod);   // жёлтый/оранжевый
                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
