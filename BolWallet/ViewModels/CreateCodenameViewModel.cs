using System.Globalization;
using Bol.Core.Abstractions;
using Bol.Core.Model;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
    private readonly ISecureRepository _secureRepository;
    private readonly RegisterContent _content;

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
        _content = content;
    }

    [ObservableProperty]
    private CodenameForm _codenameForm;

    [ObservableProperty]
    private string _codename = " ";


    [RelayCommand]
    private async Task Submit()
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

		UserData userData = new UserData();

        userData.Codename = result;
        userData.Person = person;

		await _secureRepository.SetAsync<UserData>("userdata", userData);

		await NavigationService.NavigateTo<EdiViewModel>(true);

		Codename = result;
	}

    public async Task Initialize()
    {
        var userData = await _secureRepository.GetAsync<UserData>("userdata");
        if (userData is null) return;

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
        CodenameForm.Birthdate.Value = userData.Person.Birthdate.ToString(CultureInfo.InvariantCulture);
        
        Codename = userData.Codename;
    }
}