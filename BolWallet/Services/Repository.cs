using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;

namespace BolWallet.Services;

public class Repository : IRepository
{
	private readonly IBlobCache _blobCache;
	private readonly ISecureStorage _secureStorage;

	public Repository(IBlobCache blobCache, ISecureStorage secureStorage)
	{
		_blobCache = blobCache ?? throw new ArgumentNullException(nameof(blobCache));
		_secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
	}

	public async Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class
	{
		return await _blobCache
			.GetObject<TEntity>(key)
			.Catch(Observable.Catch<TEntity>())
			.ToTask(token);
	}

	public async Task CreateAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class
	{
		if (key is null || entity is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		await _blobCache
			.InsertObject(key, entity)
			.ToTask(token);
	}

	public async Task<string> GetSecureAsync(string key)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		var result = await _secureStorage.GetAsync(key);

		return result;
	}

	public async Task<TEntity> GetSecureAsync<TEntity>(string key) where TEntity : class
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		var result = await _secureStorage.GetAsync(key);

		var entity = JsonSerializer.Deserialize<TEntity>(result);

		return entity;
	}

	public async Task CreateSecureAsync(string key, string value)
	{
		if (key is null || value is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		await _secureStorage.SetAsync(key, value);
	}

	public async Task CreateSecureAsync<TEntity>(string key, TEntity entity) where TEntity : class
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		var entityAsJson = JsonSerializer.Serialize(entity);

		await _secureStorage.SetAsync(key, entityAsJson);
	}
}