using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.Globalization;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
	private readonly ICodeNameService _codeNameService;
	private readonly ISecureRepository _secureRepository;
    public UserData _userData;
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
	private Task Submit()
	{
		return NavigationService.NavigateTo<CreateEdiViewModel>(true);
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

            _userData  = new UserData
			{
				Codename = result,
				Person = person
			};

			await _secureRepository.SetAsync("userdata", _userData);

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
        _userData = await _secureRepository.GetAsync<UserData>("userdata");
		if (_userData is null) return;

		CodenameForm.FirstName.Value = _userData.Person.FirstName;
		CodenameForm.MiddleName.Value = _userData.Person.MiddleName;
		CodenameForm.Surname.Value = _userData.Person.Surname;
		CodenameForm.ThirdName.Value = _userData.Person.ThirdName;
		CodenameForm.Gender = _userData.Person.Gender;
		CodenameForm.Combination.Value = _userData.Person.Combination;
		CodenameForm.SelectedCountry = CodenameForm
					.Countries
					.FirstOrDefault(c => c.Alpha3 == _userData.Person.CountryCode);
		CodenameForm.NIN.Value = _userData.Person.Nin;
		CodenameForm.Birthdate.Value = _userData.Person.Birthdate.ToString(CultureInfo.InvariantCulture);

		Codename = _userData.Codename;
	}
}