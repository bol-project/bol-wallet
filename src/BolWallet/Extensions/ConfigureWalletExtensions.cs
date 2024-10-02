using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Core.Services;
using Microsoft.Extensions.Options;

namespace BolWallet.Extensions;
public static class ConfigureWalletExtensions
{
	public static IServiceCollection ConfigureWalletServices(this IServiceCollection services)
	{
        // Register a custom IContextAccessor by decorating the default one defined in BoL SDK.
        services.AddTransient<WalletContextAccessor>();
        services.AddTransient<IContextAccessor, BolWalletContextAccessor>();

        services.AddTransient<IBolService, BolService>();
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

		return services;
	}
}
