using System.Globalization;

namespace BolWallet.Converters;

public class EndIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (PropertyState)value switch
        {
            PropertyState.IsReady => "\ue876",
            PropertyState.HasError => "\ue000",
            _ => ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}