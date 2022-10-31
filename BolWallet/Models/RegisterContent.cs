using Bol.Core.Model;

namespace BolWallet.Models;

public class RegisterContent
{
	public IEnumerable<Country> Countries { get; set; }
	public IDictionary<string, NinSpecification> NinPerCountryCode { get; set; }
}