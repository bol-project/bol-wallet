using System.Reflection;
using Akavache;
using Bol.Core.Extensions;
using Bol.Core.Model;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using BolWallet.Extensions;
using BolWallet.Services.BolRpc;
using BolWallet.Services.PermissionServices;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Plugin.Maui.Audio;
using Country = BolWallet.Models.Country;

namespace BolWallet;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services.ConfigureSerilog();

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

        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.AddConfiguration("BolWallet.appsettings.Development.json");
#else
        builder.AddConfiguration("BolWallet.appsettings.json");
#endif

        var services = builder.Services;

        // Register Services
        services.AddScoped<IRepository>(_ => new Repository(BlobCache.UserAccount));
        services.AddScoped<ISecureRepository, AkavacheRepository>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<ICitizenshipHashTableProcessor, CitizenshipHashTableProcessor>();
        services.AddSingleton<IFilePicker>(_ => FilePicker.Default);
        services.AddScoped<ICountriesService, CountriesService>();

        RegisterPermissionServices(services);

        services.AddSingleton(MediaPicker.Default);
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

        services.AddSingleton<IDeviceDisplay>(DeviceDisplay.Current);

        services.AddSingleton<IFileDownloadService, FileDownloadService>();

        services.RegisterViewAndViewModelSubsystem();

        services.AddSingleton(AudioManager.Current);

        // Register service to fetch the BoL Contract hash from the RPC node.
        services.AddHttpClient<IBolRpcService, BolRpcService>("BolRpcService");

        services.AddBolSdk();

        services.AddSingleton<HttpClient>();

        // This model will hold the data from the Register flow
        services.AddSingleton(sp =>
        {
            var countries = JsonSerializer.Deserialize<List<Country>>(ReadResource("BolWallet.Resources.Content.country_code.json"), Constants.WalletJsonSerializerDefaultOptions);
            var ninSpecifications = JsonSerializer.Deserialize<List<NinSpecification>>(ReadResource("BolWallet.Resources.Content.nin.json"), Constants.WalletJsonSerializerDefaultOptions);

            return new RegisterContent
            {
                Countries = countries.Select(c => new Country { Alpha3 = c.Alpha3, Name = c.Name, Region = c.Region }).ToList(),
                NinPerCountryCode = ninSpecifications.DistinctBy(c => c.CountryCode).ToDictionary(n => n.CountryCode, n => n)
            };
        });

        services.ConfigureWalletServices();

        services.AddTransient<IBase64Encoder, Base64Encoder>();
        services.AddHttpClient<IBolChallengeService, BolChallengeService>("BolChallengeService");
        
        Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit

        return builder.Build();
    }

    private static IServiceCollection RegisterPermissionServices(IServiceCollection services)
    {
        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            services.AddSingleton<IPermissionService, MacCatalystPermissionService>();
        }
        else if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            services.AddSingleton<IPermissionService, IosPermissionService>();
        }
        else
        {
            services.AddSingleton<IPermissionService, AndroidPermissionService>();
        }
        return services;
    }

    private static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder, string appSettingsPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(appSettingsPath);

        var configRoot = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(configRoot);

        var bolSettingsSection = builder.Configuration.GetRequiredSection("BolSettings");
        var mainNetBolConfig = bolSettingsSection.GetSection(Constants.MainNet).Get<BolWalletAppConfig>();
        var testNetBolConfig = bolSettingsSection.GetSection(Constants.TestNet).Get<BolWalletAppConfig>();

        builder.Services.AddSingleton<INetworkPreferences>(_ =>
            new NetworkPreferences(Preferences.Default, mainNetBolConfig, testNetBolConfig));
        
        return builder;
    }

    private static string ReadResource(string filePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(filePath);

        if (stream is null)
        {
            throw new FileNotFoundException(nameof(filePath));
        }

        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();

        return result;
    }
}
