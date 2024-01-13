using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
    private readonly ISecureRepository _secureRepository;

    public CreateCodenameViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository)
        : base(navigationService)
    {
        _codeNameService = codeNameService;
        _secureRepository = secureRepository;
        CodenameForm = new CodenameForm(content);
    }

    [ObservableProperty]
    private CodenameForm _codenameForm;

    [ObservableProperty]
    private string _codename = " ";


    [RelayCommand]
    public async Task Submit()
    {
        await App.Current.MainPage.Navigation.PushAsync(new CreateEdiPage());
    }

    [RelayCommand]
    private async Task Generate()
    {
        try
        {
            if (!CodenameForm.IsFormFilled)
            {
                return;
            }

            UserData userData = await this._secureRepository.GetAsync<UserData>("userdata");

            var person = new NaturalPerson
            {
                FirstName = CodenameForm.FirstName.Value,
                MiddleName = CodenameForm.MiddleName.Value,
                Surname = CodenameForm.Surname.Value,
                ThirdName = CodenameForm.ThirdName.Value,
                Gender = CodenameForm.Gender,
                Combination = CodenameForm.Combination.Value,
                Nin = CodenameForm.NIN.Value,
                Birthdate = DateTime.Parse(CodenameForm.Birthdate.Value),
                CountryCode = CodenameForm.SelectedCountry.Alpha3
            };

            var result = _codeNameService.Generate(person);

            if (IsCodenameExists(result))
            {
                Toast.Make("The codename already exists. Please create a different codename.").Show().Wait();
                return;
            }

            userData = new UserData
            {
                Codename = result,
                Person = person
            };

            await _secureRepository.SetAsync("userdata", userData);

            Codename = result;
            CodenameForm.IsInvalidated = false;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    public async Task Initialize()
    {
        var userData = await _secureRepository.GetAsync<UserData>("userdata");
        if (userData?.Person is null) return;

        CodenameForm.FirstName.Value = userData.Person.FirstName;
        CodenameForm.MiddleName.Value = userData.Person.MiddleName;
        CodenameForm.Surname.Value = userData.Person.Surname;
        CodenameForm.ThirdName.Value = userData.Person.ThirdName;
        CodenameForm.Gender = userData.Person.Gender;
        CodenameForm.Combination.Value = userData.Person.Combination;
        CodenameForm.SelectedCountry = CodenameForm
                    .Countries
                    .FirstOrDefault(c => c.Alpha3 == userData.Person.CountryCode);
        CodenameForm.NIN.Value = userData.Person.Nin;
        CodenameForm.Birthdate.Value = userData.Person.Birthdate.ToString("yyyy-MM-dd");

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
