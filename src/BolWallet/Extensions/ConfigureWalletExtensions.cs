using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Core.Rpc;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BolWallet.Extensions;
public static class ConfigureWalletExtensions
{
	public static IServiceCollection ConfigureWalletServices(this IServiceCollection services)
	{
        // Register a custom IContextAccessor by decorating the default one defined in BoL SDK.
        services.AddTransient<WalletContextAccessor>();
        services.AddTransient<IContextAccessor, BolWalletContextAccessor>();

        // For SDK services with a direct or indirect dependency to IOptions<BolConfig>
        // make them transient to use a new IOptions<BolConfig> instance properly and
        // avoid using an incorrect BolConfig after closing/opening wallets.
        services.AddTransient<IRpcClient, RpcClient>();
        services.AddTransient<ITransactionService, TransactionService>();
        services.AddSingleton<HttpClient>();
        
        services.AddScoped<BolService>();
        services.AddSingleton<BolServiceFactory>();
        services.AddTransient<IBolService>(sp => sp.GetRequiredService<BolServiceFactory>().Create());
        
		services.AddTransient<IOptions<WalletConfiguration>>((sp) =>
		{
			ISecureRepository secureRepository = sp.GetRequiredService<ISecureRepository>();

			var userData = secureRepository.Get<UserData>("userdata");

			return Options.Create(new WalletConfiguration { Password = userData?.WalletPassword });
		});

		services.AddTransient<IOptions<Bol.Core.Model.BolWallet>>((sp) =>
		{
			ISecureRepository secureRepository = sp.GetRequiredService<ISecureRepository>();

			var userData = secureRepository.Get<UserData>("userdata");

			return Options.Create(userData?.BolWallet ?? new Bol.Core.Model.BolWallet());
		});

        services.AddTransient<IOptions<BolConfig>>(sp =>
        {
            var networkPreferences = sp.GetRequiredService<INetworkPreferences>();
            var targetNetworkConfig = networkPreferences.TargetNetworkConfig;
            return Options.Create(new BolConfig { RpcEndpoint = targetNetworkConfig.RpcEndpoint, Contract = targetNetworkConfig.Contract });
        });

        // Re-register SDK's Cache as scoped instead of the default singleton
        // to make sure it's disposed and new BolService instances don't use the same instance.
        // This avoids loading the same BolContext as the loaded first,
        // even after closing a wallet and opening another.
        services.AddScoped<ICachingService>(provider =>
        {
            var cacheOptions = Options.Create(new MemoryCacheOptions { });

            var memoryCache = new MemoryCache(cacheOptions);

            return new CachingService(memoryCache);
        });

        return services;
	}
}
