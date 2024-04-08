using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models;

public class CitizenshipsForm : ObservableObject
{
    [Required]
    public string FirstCountry { get; set; }
    public string SecondCountry { get; set; }
    public string ThirdCountry { get; set; }
}

