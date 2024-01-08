using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

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
                    Name = CompanyCodenameForm.CompanyCountry.Name,
                    Alpha3 = CompanyCodenameForm.CompanyCountry.Alpha3,
                    Region = CompanyCodenameForm.CompanyCountry.Region
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

            var userData = new UserData
            {
                Codename = result,
                Company = company
            };

            await _secureRepository.SetAsync("userdata", userData);

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
        var userData = await _secureRepository.GetAsync<UserData>("userdata");
        if (userData?.Company is null) return;

        var country = new Models.Country
        {
            Name = userData.Company.Country.Name,
            Alpha3 = userData.Company.Country.Alpha3,
            Region = userData.Company.Country.Region
        };

        CompanyCodenameForm.CompanyCountry = country;
        CompanyCodenameForm.OrgType = userData.Company.OrgType;
        CompanyCodenameForm.Title = userData.Company.Title;
        CompanyCodenameForm.VatNumber = userData.Company.VatNumber;
        CompanyCodenameForm.IncorporationDate = userData.Company.IncorporationDate;
        CompanyCodenameForm.ExtraDigit = userData.Company.ExtraDigit;

        Codename = userData.Codename;
    }
}
