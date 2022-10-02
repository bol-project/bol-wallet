﻿using DevExpress.Maui;

namespace BolWallet;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseDevExpress()
			.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

		var services = builder.Services;

		// Register Services
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

		return builder.Build();
	}
}