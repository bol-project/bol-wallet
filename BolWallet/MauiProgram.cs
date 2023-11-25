using Bol.App.Core.Services;
using BolWallet.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using Akavache;
using Bol.Core.Extensions;
using Bol.Core.Model;
using BolWallet.Extensions;
using CommunityToolkit.Maui;
using Country = BolWallet.Models.Country;
using MudBlazor.Services;

namespace BolWallet
{
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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		//builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServices();
            builder.Services.AddSingleton<WeatherForecastService>();
            var services = builder.Services;
            // Register Services
            builder.Services.AddScoped<IRepository>(_ => new Repository(BlobCache.UserAccount));
            builder.Services.AddScoped<ISecureRepository, AkavacheRepository>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddSingleton<IPermissionService, PermissionService>();
            builder.Services.AddScoped<Breadcrumbs>();
            builder.Services.AddSingleton<IMediaPicker, Services.MediaPicker>();
            builder.Services.RegisterViewAndViewModelSubsystem();

            // Register RpcEndpoint and Contract
            var bolConfig = new BolConfig();
            builder.Configuration.GetSection("BolSettings").Bind(bolConfig);
            builder.Services.AddSingleton(typeof(IOptions<BolConfig>), Microsoft.Extensions.Options.Options.Create(bolConfig));

            builder.Services.AddBolSdk();

            using var sp = services.BuildServiceProvider();

            var countries = sp.GetRequiredService<IOptions<List<Bol.Core.Model.Country>>>().Value;
            var ninSpecifications = sp.GetRequiredService<IOptions<List<NinSpecification>>>().Value;
            var content = new RegisterContent
            {
                Countries = countries.Select(c => new Country { Alpha3 = c.Alpha3, Name = c.Name, Region = c.Region }).ToList(),
                NinPerCountryCode = ninSpecifications.ToDictionary(n => n.CountryCode, n => n)
            };

            // This model will hold the data from the Register flow
            builder.Services.AddSingleton(content);
            builder.Services.ConfigureWalletServices();

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
}
