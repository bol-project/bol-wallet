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

    [Required]
    public Bol.Core.Model.Country Country { get; set; }

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
}
