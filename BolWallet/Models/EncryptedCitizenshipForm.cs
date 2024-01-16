using System.ComponentModel.DataAnnotations;
using Bol.Core.Model;

namespace BolWallet.Models;
public class EncryptedCitizenshipForm
{
    [Required]
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    [Required]
    public string BirthCountryCode { get; set; }
    [Required]
    public string Nin { get; set; }
    [Required]
    public string SurName { get; set; }
    [Required]
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string ThirdName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    public CitizenshipHashTable CitizenshipHashes { get; set; }
}
