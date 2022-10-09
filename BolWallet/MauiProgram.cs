﻿using System.Reflection;
using Akavache;
using Bol.Core.Extensions;
using Bol.Core.Model;
using DevExpress.Maui;
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
		.UseDevExpress()
		.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
		})
		.AddConfiguration("BolWallet.appsettings.json");
		
		var services = builder.Services;

		// Register Services
		services.AddScoped<IRepository>(sp => new Repository(BlobCache.UserAccount));
		services.AddScoped<ISecureRepository>(sp => new SecureRepository(SecureStorage.Default));
		services.AddScoped<INavigationService, NavigationService>();
		services.AddScoped<ICountriesService, CountriesService>();

		// Register Pages
		services.AddTransient<MainPage>();
		services.AddTransient<CreateCodenamePage>();

		// RegisterViewModels
		services.AddTransient<MainPageViewModel>();
		services.AddTransient<CreateCodenameViewModel>();

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

		// Register Helpers - Special services
		services.AddScoped<CodenameFormDataProvider>();
		
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