using System.Text.Json;

namespace BolWallet.Services;

public class CountriesService : ICountriesService
{
	private readonly RegisterContent _registerContent;

	public CountriesService(RegisterContent registerContent)
	{
		_registerContent = registerContent;
	}
	
	public async Task<IEnumerable<Country>> GetAsync()
	{
		if (_registerContent.Countries is not null)
		{
			return _registerContent.Countries;
		}

		//await using var stream = await FileSystem.OpenAppPackageFileAsync("country_code.json");
		using var reader = new StreamReader(".content/country_code.json");
		var countryCodeJson = await reader.ReadToEndAsync();
		
		_registerContent.Countries = JsonSerializer.Deserialize<IList<Country>>(countryCodeJson);

		return _registerContent.Countries;
	}
}