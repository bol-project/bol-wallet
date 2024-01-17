using Bol.Core.Model;

namespace BolWallet.Models;
public class UserData
{
    public NaturalPerson Person { get; set; }
    public string Codename { get; set; }
    public List<Country> Citizenships { get; set; }
    public string Edi { get; set; }
    public GenericHashTable GenericHashTable { get; set; }
    public List<EncryptedCitizenship> CitizenshipMatrices { get; set; }
    public Bol.Core.Model.BolWallet BolWallet { get; set; }
    public string WalletPassword { get; set; }
    public bool IsCertifier { get; set; }
}
