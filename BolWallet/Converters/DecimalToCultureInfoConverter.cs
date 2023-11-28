using System.Globalization;

namespace BolWallet.Converters;
public class DecimalToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var decimalValue = (decimal)value;
		var ci = new CultureInfo("fr-FR");
		return decimalValue.ToString("F8", ci) + " Bol";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
