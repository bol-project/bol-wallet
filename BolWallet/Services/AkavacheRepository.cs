using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;

namespace BolWallet.Services;

public class AkavacheRepository : IRepository
{
	private readonly IBlobCache _blobCache;

	public AkavacheRepository(IBlobCache blobCache)
	{
		_blobCache = blobCache ?? throw new ArgumentNullException(nameof(blobCache));
	}

	public async Task<TEntity> GetAsync<TEntity>(string key, CancellationToken token = default)
		where TEntity : class
	{
		return await _blobCache
			.GetObject<TEntity>(key)
			.Catch(Observable.Catch<TEntity>())
			.ToTask(token);
	}

	public async Task<TEntity> CreateAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
		where TEntity : class
	{
		if (entity == null)
		{
			return null;
		}

		await _blobCache
			.InsertObject(key, entity)
			.ToTask(token);

		return entity;
	}
}