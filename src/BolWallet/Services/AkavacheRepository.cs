using Akavache;
using System.Reactive.Linq;

namespace BolWallet.Services;
public class AkavacheRepository : ISecureRepository
{
	public async Task<string> GetAsync(string key)
	{
		ValidateKey(key);

		string result;

		try
		{
			result = await BlobCache.Secure.GetObject<string>(key);
		}
		catch (KeyNotFoundException)
		{
			result = null;
		}
		return result;
	}

	public TEntity Get<TEntity>(string key) where TEntity : class
	{
		ValidateKey(key);

		TEntity result;

		try
		{
			result = BlobCache.Secure.GetObject<TEntity>(key).Wait();
		}
		catch (KeyNotFoundException)
		{
			result = null;
		}
		return result;
	}

	public async Task<TEntity> GetAsync<TEntity>(string key) where TEntity : class
	{
		ValidateKey(key);

		TEntity result;

		try
		{
			result = await BlobCache.Secure.GetObject<TEntity>(key);
		}
		catch (KeyNotFoundException)
		{
			result = null;
		}
		return result;
	}

	public async Task SetAsync(string key, string value)
	{
		ValidateKey(key);
		ValidateValue(value);

		await BlobCache.Secure.InsertObject(key, value);
	}

	public async Task SetAsync<TEntity>(string key, TEntity entity) where TEntity : class
	{
		ValidateKey(key);
		ValidateValue(entity);

		await BlobCache.Secure.InsertObject(key, entity);
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
