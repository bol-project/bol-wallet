using Bol.Core.Model;

namespace BolWallet.Models;
public class UserData
{
	public NaturalPerson Person { get; set; }
	public string Codename { get; set; }
	public string Edi { get; set; }
    public EncryptedDigitalMatrix EncryptedDigitalMatrix { get; set; }
    public Bol.Core.Model.Wallet.BolWallet BolWallet { get; set; }
}
