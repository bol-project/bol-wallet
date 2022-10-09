using DevExpress.Maui;

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
			});

		var services = builder.Services;

		// Register Services
		services.AddScoped<IRepository>(sp => new Repository(BlobCache.UserAccount, SecureStorage.Default));
		services.AddScoped<INavigationService, NavigationService>();
		services.AddScoped<ICountriesService, CountriesService>();

		// Register Pages
		services.AddTransient<MainPage>();
		services.AddTransient<CodenamePage>();

		// RegisterViewModels
		services.AddTransient<MainPageViewModel>();
		services.AddTransient<CodenameViewModel>();

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
}