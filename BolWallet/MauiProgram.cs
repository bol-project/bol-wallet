namespace BolWallet;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
		
		var services = builder.Services;
		
		// Register Services
		services.AddScoped<INavigationService, NavigationService>();
		
		// Register Pages
		services.AddTransient<MainPage>();
		services.AddTransient<CodenamePage>();
		
		// RegisterViewModels
		services.AddTransient<CodenameViewModel>();
		services.AddTransient<MainPageViewModel>();
		
		// RegisterContent(services);
		
		return builder.Build();
	}

	private static IServiceCollection RegisterContent(this IServiceCollection services)
	{
		var countries = CountriesHelper.GetCountries().Result;
		
		services.AddSingleton(new WalletAppContent
		{
			Countries = countries
		});
		
		return services;
	}
}
