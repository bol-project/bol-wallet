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
		services.AddTransient<CodenameViewModel>();
		services.AddTransient<MainPageViewModel>();

		// This model will hold the data from the Register flow
		services.AddSingleton(new RegisterContent());

		// Register Helpers - Special services
		services.AddScoped<CodenameFormDataProvider>();
		Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit

		return builder.Build();
	}
}