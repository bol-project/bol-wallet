namespace BolWallet.Services;

public interface IRepository
{
	Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default) 
		where TEntity : class;

	Task<TEntity> CreateAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class;
}