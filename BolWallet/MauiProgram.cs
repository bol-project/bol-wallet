using System.Reflection;
using Akavache;
using Bol.App.Core.Services;
using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Extensions;
using Bol.Core.Model;
using Bol.Core.Services;
using BolWallet.Extensions;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Country = BolWallet.Models.Country;
namespace BolWallet;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
			});

		builder.AddConfiguration("BolWallet.appsettings.json");

		var services = builder.Services;

		// Register Services
		services.AddScoped<IRepository>(_ => new Repository(BlobCache.UserAccount));
		services.AddScoped<ISecureRepository>(_ => new SecureRepository(SecureStorage.Default));
		services.AddSingleton<INavigationService, NavigationService>();
		services.AddScoped<ICountriesService, CountriesService>();
		services.AddSingleton<IPermissionService, PermissionService>();

		services.AddSingleton<IMediaPicker, Services.MediaPicker>();
		services.RegisterViewAndViewModelSubsystem();

		// Register RpcEndpoint and Contract
		var bolConfig = new BolConfig();
		builder.Configuration.GetSection("BolSettings").Bind(bolConfig);
		services.AddSingleton(typeof(IOptions<BolConfig>), Microsoft.Extensions.Options.Options.Create(bolConfig));

		services.AddBolSdk();

		using var sp = services.BuildServiceProvider();

		var countries = sp.GetRequiredService<IOptions<List<Bol.Core.Model.Country>>>().Value;
		var ninSpecifications = sp.GetRequiredService<IOptions<List<NinSpecification>>>().Value;
		var content = new RegisterContent
		{
			Countries = countries.Select(c => new Country { Alpha3 = c.Alpha3, Name = c.Name, Region = c.Region }).ToList(),
			NinPerCountryCode = ninSpecifications.ToDictionary(n => n.CountryCode, n => n)
		};

		// This model will hold the data from the Register flow
		services.AddSingleton(content);

		ConfigureWalletServices(services, sp);

		Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit

		return builder.Build();
	}

	private static void ConfigureWalletServices(IServiceCollection services, ServiceProvider sp)
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
	}

	private static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder, string appSettingsPath)
	{
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream(appSettingsPath);

		var configRoot = new ConfigurationBuilder()
			.AddJsonStream(stream)
			.Build();

		builder.Configuration.AddConfiguration(configRoot);

		var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

		var bolConfig = configuration.GetRequiredSection("BolSettings").Get<BolConfig>();

		builder.Services.AddSingleton(bolConfig);

		return builder;
	}
}
