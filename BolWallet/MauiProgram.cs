using System.Reflection;
using Akavache;
using Bol.App.Core.Services;
using Bol.Core.Extensions;
using Bol.Core.Model;
using Bol.Cryptography;
using Bol.Cryptography.Encoders;
using BolWallet.Extensions;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Newtonsoft.Json.Linq;
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
		services.AddScoped<ICountriesService, CountriesService>();
		services.AddSingleton<IPermissionService, PermissionService>();

        services.AddSingleton<IMediaPicker, Services.MediaPicker>();
		builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

        services.AddSingleton<IDeviceDisplay>(DeviceDisplay.Current);

        services.AddSingleton<IFileDownloadService, FileDownloadService>();

        services.RegisterViewAndViewModelSubsystem();

        var bolConfig = new BolWalletAppConfig();
        builder.Configuration.GetSection("BolSettings").Bind(bolConfig);

        string contractHash = GetContractHash(bolConfig.RpcEndpoint);

        bolConfig.Contract = contractHash;

        services.AddSingleton(typeof(IOptions<BolConfig>), Microsoft.Extensions.Options.Options.Create(bolConfig));
        services.AddSingleton(typeof(IOptions<BolWalletAppConfig>), Microsoft.Extensions.Options.Options.Create(bolConfig));

		services.AddBolSdk();

		services.AddSingleton<HttpClient>();

        // This model will hold the data from the Register flow
        services.AddSingleton(sp =>
        {
            var countries = sp.GetRequiredService<IOptions<List<Bol.Core.Model.Country>>>().Value;
            var ninSpecifications = sp.GetRequiredService<IOptions<List<NinSpecification>>>().Value;
      
            return new RegisterContent
            {
                Countries = countries.Select(c => new Country { Alpha3 = c.Alpha3, Name = c.Name, Region = c.Region }).ToList(),
                NinPerCountryCode = ninSpecifications.DistinctBy(c => c.CountryCode).ToDictionary(n => n.CountryCode, n => n)
            };
        });

        services.ConfigureWalletServices();

        services.AddTransient<IBase64Encoder, Base64Encoder>();
        services.AddHttpClient<IBolChallengeService, BolChallengeService>("BolChallengeService", client =>
        {
            client.BaseAddress = new Uri(bolConfig.BolIdentityEndpoint);
        });

		Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit

		return builder.Build();
	}

    private static string GetContractHash(string url)
    {
        try
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var stringContent = new StringContent("{\r\n\"jsonrpc\":\"2.0\",\r\n\"id\":1,\r\n\"method\":\"getbolhash\",\r\n\"params\":[]\r\n}\r\n", null, "application/json");
            request.Content = stringContent;

            using (var response = client.SendAsync(request).Result)
            {
                response.EnsureSuccessStatusCode();

                var resultJson = response.Content.ReadAsStringAsync().Result;

                var jsonResponse = JObject.Parse(resultJson);

                var contractHash = jsonResponse["result"].ToString();

                return contractHash;
            }
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message).Show().Wait();
        }

        return string.Empty;
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
