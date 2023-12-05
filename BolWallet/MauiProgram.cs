using Bol.App.Core.Services;
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
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json.Linq;
using Blazing.Mvvm;
using Blazing.Mvvm.Infrastructure;

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
            builder.Services.AddMvvmNavigation(options =>
            {
                options.HostingModel = BlazorHostingModel.Hybrid;
            });

            
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		//builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServices();

            // Register Services
            builder.Services.AddScoped<IRepository>(_ => new Repository(BlobCache.UserAccount));
            builder.Services.AddScoped<ISecureRepository, AkavacheRepository>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddSingleton<IPermissionService, PermissionService>();
            builder.Services.AddScoped<Breadcrumbs>();
            builder.Services.AddSingleton<IMediaPicker, Services.MediaPicker>();
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.RegisterViewAndViewModelSubsystem();

            // Register RpcEndpoint and Contract
            
            string contractHash = GetContractHash();

            var bolConfig = new BolConfig()
            {
                RpcEndpoint = "https://validator-1.demo.bolchain.net:443",
                Contract = contractHash
            };

            builder.Services.AddSingleton(typeof(IOptions<BolConfig>), Microsoft.Extensions.Options.Options.Create(bolConfig));

            builder.Services.AddBolSdk();

            using var sp = builder.Services.BuildServiceProvider();

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
            builder.AddConfiguration("BolWallet.appsettings.json");
            builder.Services.AddSingleton<HttpClient>();
            Registrations.Start(AppInfo.Current.Name); // TODO stop BlobCache after quit


            return builder.Build();
        }

        private static string GetContractHash()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://validator-1.demo.bolchain.net:443");

            var stringContent = new StringContent("{\r\n\"jsonrpc\":\"2.0\",\r\n\"id\":1,\r\n\"method\":\"getbolhash\",\r\n\"params\":[]\r\n}\r\n", null, "application/json");
            request.Content = stringContent;

            try
            {
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
}
