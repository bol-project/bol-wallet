namespace BolWallet.Services;

public class SecureRepository : ISecureRepository
{
	private readonly ISecureStorage _secureStorage;

	public SecureRepository(ISecureStorage secureStorage)
	{
		_secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
	}

	public async Task<string> GetAsync(string key)
	{
		ValidateKey(key);

		var result = await _secureStorage.GetAsync(key);

		return result;
	}

	public async Task<TEntity> GetAsync<TEntity>(string key) where TEntity : class
	{
		ValidateKey(key);

		var result = await _secureStorage.GetAsync(key);
        
        if (string.IsNullOrWhiteSpace(result)) return null;

        var entity = JsonSerializer.Deserialize<TEntity>(result);

		return entity;
	}

	public async Task SetAsync(string key, string value)
	{
		ValidateKey(key);
		ValidateValue(value);

		await _secureStorage.SetAsync(key, value);
	}

	public async Task SetAsync<TEntity>(string key, TEntity entity) where TEntity : class
	{
		ValidateKey(key);
		ValidateValue(entity);

        var entityAsJson = JsonSerializer.Serialize(entity);

		await _secureStorage.SetAsync(key, entityAsJson);
	}

	private static void ValidateKey(string key)
	{
		if (key is null) throw new ArgumentNullException(nameof(key));
	}

	private static void ValidateValue(object value)
	{
		if (value is null) throw new ArgumentNullException(nameof(value));
	}
}