using BolWallet.Services;
using BolWallet.ViewModels;

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
		
		// Register Services
		builder.Services.AddScoped<INavigationService, NavigationService>();
		
		// Register Pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<CodenamePage>();
		
		// RegisterViewModels
		builder.Services.AddTransient<CodenameViewModel>();
		builder.Services.AddTransient<MainPageViewModel>();
		
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
