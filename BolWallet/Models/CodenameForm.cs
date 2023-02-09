using Bol.Core.Model;
using System.Text.RegularExpressions;

namespace BolWallet.Models;

[INotifyPropertyChanged]
public partial class CodenameForm
{
    private static readonly Regex OneOrMoreCapitalLetters = new ("^[A-Z]+$");
    private static readonly Regex ZeroOrMoreCapitalLetters = new ("^[A-Z]*$");
    private static readonly Regex OneCapitalLetterOrDigit = new ("^[A-Z0-9]$");

    private readonly RegisterContent _content;

    public CodenameForm(RegisterContent content)
    {
        _content = content;
    }

    public BaseProperty FirstName { get; set; } = new()
    {
        ErrorMessage = "First Name should be only in capital letters",
        IsValid = value => OneOrMoreCapitalLetters.IsMatch(value),
        Value = "",
        IsMandatory = true
    };

    public BaseProperty Surname { get; set; } = new()
    {
        ErrorMessage = "Surname should be only in capital letters",
        IsValid = value => OneOrMoreCapitalLetters.IsMatch(value),
        IsMandatory = true
    };

    public BaseProperty MiddleName { get; set; } = new()
    {
        ErrorMessage = "Middle Name should be only in capital letters",
        IsValid = value => ZeroOrMoreCapitalLetters.IsMatch(value),
    };

    public BaseProperty ThirdName { get; set; } = new()
    {
        ErrorMessage = "Third Name should be only in capital letters",
        IsValid = value => ZeroOrMoreCapitalLetters.IsMatch(value)
    };
    
    public BaseProperty Birthdate { get; set; } = new()
    {
        ErrorMessage = "Birthdate cannot be in the future or in the current year",
        IsValid = value =>
        {
            var date = DateTime.Parse(value);
            return date.Year.CompareTo(DateTime.Today.Year) < 0;
        },
        IsMandatory = true
    };

    public Gender Gender { get; set; }

    public BaseProperty Combination { get; set; } = new()
    {
        ErrorMessage = "Combination should be a capital letter or a digit",
        IsValid = value => OneCapitalLetterOrDigit.IsMatch(value),
        Value = "1",
        IsMandatory = true
    };

    public BaseProperty NIN { get; set; } = new()
    {
        IsMandatory = true,
    };

    public IEnumerable<Country> Countries => _content.Countries;

    private Country _selectedCountry;
    public Country SelectedCountry
    {
        get => _selectedCountry;
        set
        {
            SetProperty(ref _selectedCountry, value);
            NIN.HelpMessage = _content.NinPerCountryCode[SelectedCountry.Alpha3].InternationalName;
            NIN.IsEnabled = !string.IsNullOrEmpty(SelectedCountry.Alpha3);
            NIN.IsValid = value => _content.NinPerCountryCode[SelectedCountry.Alpha3].Digits == value.Length;
            NIN.ErrorMessage = $"National Identification Number (NIN) does not match length for country {SelectedCountry.Alpha3}.";
        }
    }

    public bool IsFormFilled
    {
        get
        {
            return FirstName.IsReady &&
                   Surname.IsReady &&
                   MiddleName.IsReady &&
                   ThirdName.IsReady &&
                   Birthdate.IsReady &&
                   Combination.IsReady &&
                   NIN.IsReady;
        }
    }
}