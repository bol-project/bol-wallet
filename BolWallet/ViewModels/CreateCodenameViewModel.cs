using System.Globalization;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    protected readonly ICodeNameService _codeNameService;
    protected readonly ISecureRepository _secureRepository;
    private readonly IOptions<BolConfig> _bolConfig;
    
    public CreateCodenameViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository,
        IOptions<BolConfig> bolConfig)
        : base(navigationService)
    {
        _codeNameService = codeNameService;
        _secureRepository = secureRepository;
        _bolConfig = bolConfig;
    }

    [ObservableProperty]
    protected string _codename = " ";



    [RelayCommand]
    public async Task Submit()
    {
        if (userData.IsIndividualRegistration)
            await NavigationService.NavigateTo<CreateEdiViewModel>(true);
        else
            await NavigationService.NavigateTo<CreateCompanyEdiViewModel>(true);
    }

    protected bool IsCodenameExists(string codename)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, _bolConfig.Value.RpcEndpoint);

        var stringContent = new StringContent("{\r\n\"jsonrpc\":\"2.0\",\r\n\"id\":1,\r\n\"method\":\"getAccount\",\r\n\"params\":[\"" + codename + "\"]\r\n}\r\n", null, "application/json");
        request.Content = stringContent;

        try
        {
            using (var response = client.SendAsync(request).Result)
            {
                response.EnsureSuccessStatusCode();

                var resultJson = response.Content.ReadAsStringAsync().Result;

                var jsonResponse = JObject.Parse(resultJson);

                if (jsonResponse["error"] == null)
                    return true;

            }
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message).Show().Wait();

            return false;
        }

        return false;
    }

    protected static DateTime GetBirthDate(string value) =>
        DateOnly.ParseExact(value, Constants.BirthDateFormat, CultureInfo.InvariantCulture)
            .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
}
