using System.Reflection;
using System.Runtime.Intrinsics.X86;
using Akavache;
using Bol.App.Core.Services;
using Bol.Core.Abstractions;
using Bol.Core.Extensions;
using Bol.Core.Helpers;
using Bol.Core.Model;
using Bol.Core.Serializers;
using Bol.Core.Services;
using Bol.Core.Validators;
using BolWallet.Extensions;
using CommunityToolkit.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using Country = BolWallet.Models.Country;

namespace BolWallet;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
		.UseMauiApp<App>()
		.UseDevExpress()
		.UseMauiCommunityToolkit()
		.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
		})
		.AddConfiguration("BolWallet.appsettings.json");
		
		var services = builder.Services;

		// Register Services
		services.AddScoped<IRepository>(_ => new Repository(BlobCache.UserAccount));
		services.AddScoped<ISecureRepository>(_ => new SecureRepository(SecureStorage.Default));
		services.AddSingleton<INavigationService, NavigationService>();
		services.AddScoped<ICountriesService, CountriesService>();
		services.AddSingleton<IPermissionService, PermissionService>();




		services.AddSingleton<ISerializer, Serializer>();
		services.AddSingleton<IDeserializer, Deserializer>();
		services.AddSingleton<IYamlSeralizer, YamlSerializer>();
		services.AddSingleton<IHashTableValidator, HashTableValidator>();
		services.AddSingleton<IYamlSeralizer, YamlSerializer>();
		services.AddSingleton<IRegexHelper, RegexHelper>();
		services.AddSingleton<ICodeNameValidator, CodeNameValidator>();
		services.AddSingleton<IEncryptedDigitalMatrixValidator, EncryptedDigitalMatrixValidator>();
		services.AddSingleton<IEncryptedDigitalIdentityService, EncryptedDigitalIdentityService>();

		services.AddSingleton<IMediaPicker, Services.MediaPicker>();
		services.RegisterViewAndViewModelSubsystem();

		services.AddBolSdk();

		using var sp = services.BuildServiceProvider();

		var countries = sp.GetRequiredService<IOptions<List<Bol.Core.Model.Country>>>().Value;
		var ninSpecifications = sp.GetRequiredService<IOptions<List<NinSpecification>>>().Value;
		var content = new RegisterContent
		{
			Countries = countries.Select(c => new Country{Alpha3 = c.Alpha3, Name = c.Name, Region = c.Region}).ToArray(),
			NinPerCountryCode = ninSpecifications.ToDictionary(n => n.CountryCode, n => n)
		};

		// This model will hold the data from the Register flow
		services.AddSingleton(content);

		Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit

		return builder.Build();
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