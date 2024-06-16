using System.Text.Json.Serialization;

namespace BolWallet.Models;

public class Country
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
	[JsonPropertyName("alpha3")]
	public string Alpha3 { get; set; }
	[JsonPropertyName("region")]
	public string Region { get; set; }
}