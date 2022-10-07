namespace BolWallet.Services;

public interface IRepository
{
	Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class;

	Task CreateAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class;

	/// <summary>
	/// Gets the value of this <see cref="key"/> from the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task<string> GetSecureAsync(string key);
	
	/// <summary>
	/// Gets the deserialized value of this <see cref="key"/> from the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task<TEntity> GetSecureAsync<TEntity>(string key) where TEntity : class;

	/// <summary>
	/// Saves the key-value pair to the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task CreateSecureAsync(string key, string value);
	
	/// <summary>
	/// Saves the key-value pair to the secure encrypted storage.
	/// The <see cref="entity"/> is saved as a json string.
	/// </summary>
	/// <returns></returns>
	Task CreateSecureAsync<TEntity>(string key, TEntity entity) where TEntity : class;
}