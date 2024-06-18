namespace BolWallet.Bolnformation;
public static class CountriesNotSupportedInformation
{
    public static string GetTitle(string country)
    {
        return $"{country} NIN Not Supported";
    }

    public static string GetDescription(string country)
    {
        return $"Currently, we do not have information regarding the National Identification Number (NIN) for '{country}'. " +
        "Please visit our website to request or submit NIN details for this country.";
    }

    public const string Content =
        "For more information check out " +
        "<a href='https://1bol.org/' target='_blank' style='color: blue; text-decoration: underline;'>BOL's official website</a>.";
}

