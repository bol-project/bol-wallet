using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Core.Services;
using Microsoft.Extensions.Options;

namespace BolWallet.Extensions;
public static class ConfigureWalletExtensions
{
	public static IServiceCollection ConfigureWalletServices(this IServiceCollection services, ServiceProvider sp)
	{
		ISecureRepository secureRepository = sp.GetRequiredService<ISecureRepository>();
		UserData userData = null;
		Task.Run(async () => userData = await secureRepository.GetAsync<UserData>("userdata")).Wait();

		if (userData?.BolWallet is not null)
		{
			services.AddSingleton(typeof(IOptions<WalletConfiguration>), Microsoft.Extensions.Options.Options.Create(new WalletConfiguration { Password = userData.WalletPassword }));
			services.AddSingleton(typeof(IOptions<Bol.Core.Model.BolWallet>), Microsoft.Extensions.Options.Options.Create(userData.BolWallet));

			services.AddSingleton<IContextAccessor, WalletContextAccessor>();
			services.AddSingleton<IBolService, BolService>();
		}

		return services;
	}
}
