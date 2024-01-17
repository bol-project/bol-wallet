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

        FirstName.PropertyChanged += (sender, args) => Invalidate();
        Surname.PropertyChanged += (sender, args) => Invalidate();
        MiddleName.PropertyChanged += (sender, args) => Invalidate();
        ThirdName.PropertyChanged += (sender, args) => Invalidate();
        Birthdate.PropertyChanged += (sender, args) => Invalidate();
        Combination.PropertyChanged += (sender, args) => Invalidate();
        NIN.PropertyChanged += (sender, args) => Invalidate();

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

    private Gender _gender;
    public Gender Gender
    {
        get => _gender;
        set
        {
            _gender = value;
            OnPropertyChanged();
            Invalidate();
        }
    }

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
        IsEnabled = true
    };

    public List<Country> Countries => _content.Countries;

    private Country _selectedCountry;
    public Country SelectedCountry
    {
        get => _selectedCountry;
        set
        {
            SetProperty(ref _selectedCountry, value);
            NIN.HelpMessage = _content.NinPerCountryCode[SelectedCountry.Alpha3].InternationalName;
            NIN.IsEnabled = string.IsNullOrEmpty(SelectedCountry.Alpha3);
            NIN.IsValid = value => new Regex(@"^[A-F0-9]+$").IsMatch(value) && _content.NinPerCountryCode[SelectedCountry.Alpha3].Digits == value.Length;
            NIN.ErrorMessage = $"National Identification Number (NIN) does not match length for country {SelectedCountry.Alpha3}. Only capital letters(A-F) and numbers accepted";
            OnPropertyChanged(nameof(NIN));
            Invalidate();
        }
    }

    private Country _countryOfBirth;
    public Country CountryOfBirth
    {
        get => _countryOfBirth;
        set
        {
            SetProperty(ref _countryOfBirth, value);
            Invalidate();
        }
    }

    public bool IsFormFilled =>
        FirstName.IsReady &&
        Surname.IsReady &&
        MiddleName.IsReady &&
        ThirdName.IsReady &&
        Birthdate.IsReady &&
        Combination.IsReady &&
        NIN.IsReady &&
        SelectedCountry is not null;

    private bool _isInvalidated = true;
    public bool IsInvalidated
    {
        get => _isInvalidated;
        set
        {
            _isInvalidated = value;
            OnPropertyChanged();
        }
    }

    private void Invalidate()
    {
        IsInvalidated = true;
        OnPropertyChanged(nameof(IsFormFilled));
        OnPropertyChanged(nameof(IsInvalidated));
    }
}
