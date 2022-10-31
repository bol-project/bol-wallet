using System.Globalization;

namespace BolWallet.Converters;

public class StateColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (PropertyState)value switch
        {
            PropertyState.IsReady => Colors.Green,
            PropertyState.HasError => Colors.Red,
            _ => Color.FromArgb("#0096FF")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}