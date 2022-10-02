using System.Text.Json;

namespace BolWallet.Services;

public class CountriesService : ICountriesService
{
	private readonly RegisterContent _registerContent;

	public CountriesService(RegisterContent registerContent)
	{
		_registerContent = registerContent;
	}
	
	// TODO make this method async
	public IEnumerable<Country> Get()
	{
		if (_registerContent.Countries is not null)
		{
			return _registerContent.Countries;
		}
		
		using var reader = new StreamReader(".content/country_code.json");
		var countryCodeJson = reader.ReadToEnd();

		_registerContent.Countries = JsonSerializer.Deserialize<IList<Country>>(countryCodeJson);
		
		return _registerContent.Countries;
	}
}