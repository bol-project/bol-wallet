namespace BolWallet.Services;

public interface IRepository
{
	Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class;

	Task CreateAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class;

	Task<string> GetSecureAsync(string key);

	Task CreateSecureAsync(string key, string value);
}