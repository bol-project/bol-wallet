using Bol.Core.Model;
using System.Text.RegularExpressions;

namespace BolWallet.Models;

[INotifyPropertyChanged]
public partial class CodenameForm
{
    private readonly RegisterContent _content;

    public CodenameForm(RegisterContent content)
    {
        _content = content;
    }

    public BaseProperty FirstName { get; set; } = new()
    {
        ErrorMessage = "First Name should be only in capital letters",
        IsValid = (value) =>
        {
            var regex = new Regex("^[A-Z]+$");
            return regex.IsMatch(value);
        },
        Value = "",
        IsMandatory = true
    };

    public BaseProperty Surname { get; set; } = new()
    {
        ErrorMessage = "Surname should be only in capital letters",
        IsValid = (value) =>
        {
            var regex = new Regex("^[A-Z]+$");
            return regex.IsMatch(value);
        },
        IsMandatory = true
    };

    public BaseProperty MiddleName { get; set; } = new()
    {
        ErrorMessage = "Middle Name should be only in capital letters",
        IsValid = (value) =>
        {
            var regex = new Regex("^[A-Z]*$");
            return regex.IsMatch(value);
        }
    };

    public BaseProperty ThirdName { get; set; } = new()
    {
        ErrorMessage = "Third Name should be only in capital letters",
        IsValid = (value) =>
        {
            var regex = new Regex("^[A-Z]*$");
            return regex.IsMatch(value);
        }
    };

    public BaseProperty Birthdate { get; set; } = new()
    {
        ErrorMessage = "Birthdate cannot be in the future",
        IsValid = (value) =>
        {
            var date = DateTime.Parse(value);
            return date.CompareTo(DateTime.Today) < 0;
        },
        IsMandatory = true
    };

    public Gender Gender { get; set; }

    public BaseProperty Combination { get; set; } = new()
    {
        ErrorMessage = "Combination should be a capital letter or a number",
        IsValid = (value) =>
        {
            var regex = new Regex("^[A-Z0-9]$");
            return regex.IsMatch(value);
        },
        Value = "1",
        IsMandatory = true
    };

    public BaseProperty NIN { get; set; } = new()
    {
        IsValid = (value) => true,
        HelpMessage = "",
        IsMandatory = true
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
        }
    }
}