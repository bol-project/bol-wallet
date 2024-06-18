namespace BolWallet.Services.Abstractions;

public interface IRepository
{
	/// <summary>
	/// Gets the value of this key from the storage.
	/// </summary>
	/// <returns></returns>
	Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class;

	/// <summary>
	/// Saves this key-value pair to the storage.
	/// </summary>
	/// <returns></returns>
	Task SetAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class;
}
