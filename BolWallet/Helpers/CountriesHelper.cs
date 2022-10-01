using System.Text.Json;
using BolWallet.Models;

namespace BolWallet.Helpers;

public class CountriesHelper
{
	public static async ValueTask<IList<Country>> GetCountries()
	{
		using var reader = new StreamReader("BolWallet/content/country_code.json");
		var countryCodeJson = await reader.ReadToEndAsync();

		return JsonSerializer.Deserialize<IList<Country>>(countryCodeJson);
	}
}