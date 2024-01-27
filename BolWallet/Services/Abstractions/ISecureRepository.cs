namespace BolWallet.Services.Abstractions;

public interface ISecureRepository
{
	TEntity Get<TEntity>(string key) where TEntity : class;

	/// <summary>
	/// Gets the value of this key from the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task<string> GetAsync(string key);
	
	/// <summary>
	/// Gets the deserialized value of this key from the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task<TEntity> GetAsync<TEntity>(string key) where TEntity : class;

	/// <summary>
	/// Saves the key-value pair to the secure encrypted storage.
	/// </summary>
	/// <returns></returns>
	Task SetAsync(string key, string value);
	
	/// <summary>
	/// Saves the key-value pair to the secure encrypted storage.
	/// The entity is saved as a json string.
	/// </summary>
	/// <returns></returns>
	Task SetAsync<TEntity>(string key, TEntity entity) where TEntity : class;
}
