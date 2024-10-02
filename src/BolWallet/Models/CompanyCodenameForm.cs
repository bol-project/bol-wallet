using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Bol.Core.Model;

namespace BolWallet.Models;

public partial class CompanyCodenameForm : ObservableObject
{
    private static readonly Regex OneOrMoreSpaces = new("\\s+");
    private static readonly Regex OneCapitalLetterOrDigit = new("^[A-Z0-9]$");

    private readonly RegisterContent _content;
    public CompanyCodenameForm(RegisterContent content)
    {
        _content = content;

        Title.PropertyChanged += (sender, args) => Invalidate();
        IncorporationDate.PropertyChanged += (sender, args) => Invalidate();
        Combination.PropertyChanged += (sender, args) => Invalidate();
        VatNumber.PropertyChanged += (sender, args) => Invalidate();
    }

    public List<Country> Countries => _content.Countries;

    public Country _companyCountry;
    [Required]
    public Country CompanyCountry
    {
        get => _companyCountry;
        set
        {
            SetProperty(ref _companyCountry, value);
            Invalidate();
        }
    }

    [Required]
    public OrgType OrgType { get; set; }

    public BaseProperty Title { get; set; } = new()
    {
        ErrorMessage = "Title must contain two or more words",
        IsValid = value => OneOrMoreSpaces.IsMatch(value),
        Value = "",
        IsMandatory = true,
        HelpMessage = "Title must contain two or more words"
    };


    public BaseProperty VatNumber { get; set; } = new()
    {
        ErrorMessage = "Provide the last 5 characters of the VAT.",
        IsValid = value => value.Length == 5,
        IsMandatory = true,
        HelpMessage = "Provide the last 5 characters of the VAT."
    };
    

    public BaseProperty IncorporationDate { get; set; } = new()
    {
        ErrorMessage = "Incorporation Date cannot be in the future",
        IsValid = value =>
        {
            var date = DateTime.Parse(value);
            return date.CompareTo(DateTime.Today) <= 0;
        },
        IsMandatory = true,
        HelpMessage = "Incorporation Date cannot be in the future."
    };

    public BaseProperty Combination { get; set; } = new()
    {
        ErrorMessage = "Combination should be a capital letter or a digit",
        IsValid = value => OneCapitalLetterOrDigit.IsMatch(value),
        Value = "1",
        IsMandatory = true,
        HelpMessage = "Combination should be a capital letter or a digit"
    };

    public bool IsFormFilled =>
        CompanyCountry is not null &&
        Title.IsReady &&
        VatNumber.IsReady &&
        IncorporationDate.IsReady &&
        Combination.IsReady;

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
