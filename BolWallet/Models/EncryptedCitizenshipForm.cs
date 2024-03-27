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

    private string _surName;

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string SurName
    {
        get => _surName;
        set => _surName = value?.ToUpper();
    }

    private string _firstName;

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string FirstName
    {
        get => _firstName;
        set => _firstName = value?.ToUpper();
    }

    private string _secondName;

    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string SecondName
    {
        get => _secondName;
        set => _secondName = value?.ToUpper();
    }

    private string _thirdName;

    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string ThirdName
    {
        get => _thirdName;
        set => _thirdName = value?.ToUpper();
    }

    public CitizenshipHashTable CitizenshipHashes { get; set; }
    public CitizenshipHashTable CitizenshipActualBytes { get; set; }

    public CitizenshipHashTableFileNames CitizenshipHashTableFileNames { get; set; }

    public bool IsSubmitted { get; set; }
}

public class CitizenshipHashTableFileNames
{
    public string IdentityCard { get; set; }

    public string IdentityCardBack { get; set; }

    public string Passport { get; set; }

    public string ProofOfNin { get; set; }

    public string BirthCertificate { get; set; }
}
