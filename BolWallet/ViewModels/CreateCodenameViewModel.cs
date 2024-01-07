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
		IndividualCodenameForm = new IndividualCodenameForm(content);
	}

	[ObservableProperty]
	private IndividualCodenameForm _individualCodenameForm;

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
			if (!IndividualCodenameForm.IsFormFilled)
			{
				return;
			}

			var person = new NaturalPerson
			{
				FirstName = IndividualCodenameForm.FirstName.Value,
				MiddleName = IndividualCodenameForm.MiddleName.Value,
				Surname = IndividualCodenameForm.Surname.Value,
				ThirdName = IndividualCodenameForm.ThirdName.Value,
				Gender = IndividualCodenameForm.Gender,
				Combination = IndividualCodenameForm.Combination.Value,
				Nin = IndividualCodenameForm.NIN.Value,
				Birthdate = DateTime.Parse(IndividualCodenameForm.Birthdate.Value),
				CountryCode = IndividualCodenameForm.SelectedCountry.Alpha3
			};

			var result = _codeNameService.Generate(person);

			if (IsCodenameExists(result))
			{
                Toast.Make("The codename already exists. Please create a different codename.").Show().Wait();
				return;
            }

            var userData = new UserData
			{
				Codename = result,
				Person = person
			};

			await _secureRepository.SetAsync("userdata", userData);

			Codename = result;
            IndividualCodenameForm.IsInvalidated = false;
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

	public async Task Initialize()
	{
		var userData = await _secureRepository.GetAsync<UserData>("userdata");
		if (userData is null) return;

        IndividualCodenameForm.FirstName.Value = userData.Person.FirstName;
        IndividualCodenameForm.MiddleName.Value = userData.Person.MiddleName;
        IndividualCodenameForm.Surname.Value = userData.Person.Surname;
        IndividualCodenameForm.ThirdName.Value = userData.Person.ThirdName;
        IndividualCodenameForm.Gender = userData.Person.Gender;
        IndividualCodenameForm.Combination.Value = userData.Person.Combination;
        IndividualCodenameForm.SelectedCountry = IndividualCodenameForm
                    .Countries
					.FirstOrDefault(c => c.Alpha3 == userData.Person.CountryCode);
        IndividualCodenameForm.NIN.Value = userData.Person.Nin;
        IndividualCodenameForm.Birthdate.Value = userData.Person.Birthdate.ToString("yyyy-MM-dd");

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