using Bol.Core.Abstractions;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json.Linq;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    protected readonly ICodeNameService _codeNameService;
    protected readonly ISecureRepository _secureRepository;

    public CreateCodenameViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository)
        : base(navigationService)
    {
        _codeNameService = codeNameService;
        _secureRepository = secureRepository;
    }

    [ObservableProperty]
    protected string _codename = " ";



    [RelayCommand]
    public async Task Submit()
    {
        await NavigationService.NavigateTo<CreateEdiViewModel>(true);
    }

    protected static bool IsCodenameExists(string codename)

		Codename = userData.Codename;
	}


    private static bool IsCodenameExists(string codename)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://rpcnode.demo.bolchain.net:443");

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
}
