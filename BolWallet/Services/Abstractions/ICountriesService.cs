namespace BolWallet.Services.Abstractions
{
    public interface ICountriesService
    {
        public Task<IEnumerable<Country>> GetAsync();
    }
}