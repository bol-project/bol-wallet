namespace BolWallet.Models;

public class RegisterContent
{
	public IEnumerable<Country> Countries { get; set; }
	public string Codename { get; set; }
	public IDictionary<string, string> Edi { get; set; }
}