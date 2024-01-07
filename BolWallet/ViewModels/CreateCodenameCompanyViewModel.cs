using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BolWallet.ViewModels;

public partial class CreateCodenameCompanyViewModel : CreateCodenameViewModel
{
    public CreateCodenameCompanyViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository) : base(navigationService, codeNameService, content, secureRepository)
    {
        CompanyCodenameForm = new CompanyCodenameForm(content);
    }

    [ObservableProperty]
    private CompanyCodenameForm _companyCodenameForm;

    [RelayCommand]
    private async Task Generate()
    {
        try
        {
            if (!CompanyCodenameForm.IsFormFilled)
            {
                return;
            }

            var company = new Company
            {
                Country = new Bol.Core.Model.Country
                {
                    Name = CompanyCodenameForm.Country.Name,
                    Alpha3 = CompanyCodenameForm.Country.Alpha3,
                    Region = CompanyCodenameForm.Country.Region
                },
                OrgType = CompanyCodenameForm.OrgType,
                Title = CompanyCodenameForm.Title,
                VatNumber = CompanyCodenameForm.VatNumber,
                IncorporationDate = CompanyCodenameForm.IncorporationDate,
                ExtraDigit = CompanyCodenameForm.ExtraDigit
            };

            var result = _codeNameService.Generate(company);

            if (IsCodenameExists(result))
            {
                Toast.Make("The codename already exists. Please create a different codename.").Show().Wait();
                return;
            }

            /*
            
            var userData = new UserData
            {
                Codename = result,
                Person = person
            };
            

            await _secureRepository.SetAsync("userdata", userData);

            */
            Codename = result;
            CompanyCodenameForm.IsInvalidated = false;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    public async Task Initialize()
    {
        /*
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
        */
    }
}
