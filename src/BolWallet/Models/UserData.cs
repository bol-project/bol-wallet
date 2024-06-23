using Bol.Core.Model;

namespace BolWallet.Models;
public class UserData
{
    public NaturalPerson Person { get; set; }
    public Company Company { get; set; }
    public string Codename { get; set; }
    public List<Country> Citizenships { get; set; }
    public string Edi { get; set; }
    public string BirthCountryCode { get; set; }
    public List<EncryptedCitizenshipData> EncryptedCitizenshipForms { get; set; } = [];
    public Bol.Core.Model.BolWallet BolWallet { get; set; }
    public GenericHashTableFiles GenericHashTableFiles { get; set; }
    public CompanyHashFiles CompanyHashFiles { get; set; }
    public string CertificationMatrix { get; set; }
    public string IdentificationMatrix { get; set; }
    public string[] CitizenshipMatrices { get; set; }
    public string CertificationMatrixCompany { get; set; }
    public string IdentificationMatrixCompany { get; set; }
    public string IncorporationMatrix { get; set; }
    public string WalletPassword { get; set; }
    public bool IsCertifier { get; set; }
    public bool IsIndividualRegistration { get; set; }
    public bool UseMainnet { get; set; }

    public string GetShortHash()
    {
        return this.Codename?.Split('<')[7];
    }
}
