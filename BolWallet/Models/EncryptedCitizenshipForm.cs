using System.ComponentModel.DataAnnotations;
using Bol.Core.Model;

namespace BolWallet.Models;
public class EncryptedCitizenshipForm
{
    [Required]
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    [Required]
    public string Nin { get; set; }

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string SurName { get; set; }

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string FirstName { get; set; }

    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string SecondName { get; set; }

    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string ThirdName { get; set; }

    public CitizenshipHashTable CitizenshipHashes { get; set; }
}
