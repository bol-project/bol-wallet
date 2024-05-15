using System.Text.RegularExpressions;
using Bol.Core.Model;

namespace BolWallet.Helpers;

public interface INinHelper
{
    /// <summary>
    ///  Validates the National Identification Number (NIN) based on the country code.
    /// </summary>
    /// <param name="nin"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    (bool isValid, string errorMessage) ValidateNin(string nin, string countryCode);
    
    /// <summary>
    /// Returns the international name of the National Identification Number (NIN) based on the country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    string GetNinInternationalName(string countryCode);
}

public class NinHelper : INinHelper
{
    private readonly RegisterContent _registerContent;

    public NinHelper(RegisterContent registerContent)
    {
        _registerContent = registerContent;
    }
    
    public (bool isValid, string errorMessage) ValidateNin(string nin, string countryCode)
    {
        if (nin is null)
        {
            return (false, "NIN cannot be null.");
        }
        
        const string Pattern = @"^[A-Z0-9]*$";
        var regex = new Regex(Pattern);

        if (string.IsNullOrWhiteSpace(countryCode) ||
            !_registerContent.NinPerCountryCode.TryGetValue(countryCode, out NinSpecification ninSpec))
        {
            return (false, "Invalid country code or country code does not exist.");
        }

        var ninRequiredDigits = ninSpec.Digits;
    
        bool isNinValid = regex.IsMatch(nin);
        bool isNinLengthCorrect = ninRequiredDigits == nin.Length;

        if (isNinValid && isNinLengthCorrect)
        {
            return (true, "");
        }

        return (false,
            $"The National Identification Number (NIN) provided does not match" +
            $" the expected length of {ninRequiredDigits} digits for the country code {countryCode}." +
            " Please ensure that only capital letters (A-Z) and numbers are used for NIN.");
    }

    public string GetNinInternationalName(string countryCode)
    {
        NinSpecification nin = null;
        bool isCountryCodeValidAndPresent = !string.IsNullOrWhiteSpace(countryCode) &&
                                            _registerContent.NinPerCountryCode.TryGetValue(countryCode, out nin);

        return isCountryCodeValidAndPresent ? nin.InternationalName : "";
    }
}
