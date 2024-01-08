using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class CreateCodenameIndividualViewModel : CreateCodenameViewModel
{
    public CreateCodenameIndividualViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository) : base(navigationService, codeNameService, content, secureRepository)
    {
        IndividualCodenameForm = new IndividualCodenameForm(content);
    }

    [ObservableProperty]
    private IndividualCodenameForm _individualCodenameForm;

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
                Person = person,
                IsIndividualRegistration = true
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
        if (userData?.Person is null) return;

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
}

