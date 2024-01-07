using System.ComponentModel.DataAnnotations;
using Bol.Core.Model;

namespace BolWallet.Models;

public partial class CompanyCodenameForm : ObservableObject
{
    private readonly RegisterContent _content;

    public CompanyCodenameForm(RegisterContent content)
    {
        _content = content;
    }

    public List<Country> Countries => _content.Countries;

    [Required]
    public Country Country { get; set; }

    [Required]
    public OrgType OrgType { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string VatNumber { get; set; }

    [Required]
    public DateTime IncorporationDate { get; set; }

    [Required]
    public int ExtraDigit { get; set; }

    public bool IsFormFilled =>
        Country is not null &&
        //OrgType condition if applied &&
        Title is not null &&
        VatNumber is not null &&
        IncorporationDate <= DateTime.Today &&
        ExtraDigit > -1;

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
