namespace BolWallet.Services
{
	public interface ICountriesService
	{
		public Task<IEnumerable<Country>> GetAsync();
	}
}