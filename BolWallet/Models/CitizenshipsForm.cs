using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models;
internal class CitizenshipsForm
{

    [Required]
    public string Citizenships { get; set; }
}
