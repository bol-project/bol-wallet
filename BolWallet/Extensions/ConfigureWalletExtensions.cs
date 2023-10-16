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
		services.AddTransient<IContextAccessor, WalletContextAccessor>();

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

			return Options.Create(userData?.BolWallet);
		});

		return services;
	}
}
