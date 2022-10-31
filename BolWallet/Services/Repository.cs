using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;

namespace BolWallet.Services;

public class Repository : IRepository
{
	private readonly IBlobCache _blobCache;

	public Repository(IBlobCache blobCache)
	{
		_blobCache = blobCache ?? throw new ArgumentNullException(nameof(blobCache));
	}

	public async Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class
	{
		ValidateKey(key);
		
		return await _blobCache
			.GetObject<TEntity>(key)
			.Catch(Observable.Catch<TEntity>())
			.ToTask(token);
	}

	public async Task SetAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class
	{
		ValidateKey(key);
		ValidateValue(entity);

		await _blobCache
			.InsertObject(key, entity)
			.ToTask(token);
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