using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;

namespace BolWallet.ViewModels;

public partial class CreateCodenameCompanyViewModel : CreateCodenameViewModel
{
    public CreateCodenameCompanyViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository,
        IOptions<BolConfig> bolConfig) : base(navigationService, codeNameService, content, secureRepository, bolConfig)
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

            userData = await this._secureRepository.GetAsync<UserData>("userdata") ?? new UserData();

            var birthDate = DateTime.Parse(CompanyCodenameForm.IncorporationDate.Value);

            var company = new Company
            {
                Country = new Bol.Core.Model.Country
                {
                    Name = CompanyCodenameForm.CompanyCountry.Name,
                    Alpha3 = CompanyCodenameForm.CompanyCountry.Alpha3,
                    Region = CompanyCodenameForm.CompanyCountry.Region
                },
                OrgType = CompanyCodenameForm.OrgType,
                Title = CompanyCodenameForm.Title.Value,
                VatNumber = CompanyCodenameForm.VatNumber.Value,
                IncorporationDate = new DateTime(birthDate.Year, birthDate.Month, birthDate.Day, 12, 0, 0, DateTimeKind.Utc),
                Combination = CompanyCodenameForm.Combination.Value
            };

            var result = _codeNameService.Generate(company);

            if (IsCodenameExists(result))
            {
                Toast.Make("The codename already exists. Please create a different codename.").Show().Wait();
                return;
            }

            userData.Codename = result;
            userData.Company = company;
            userData.IsIndividualRegistration = false;

            await _secureRepository.SetAsync("userdata", userData);

            Codename = result;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    public static string EnumDisplayedName(Bol.Core.Model.OrgType orgType)
    {
        return orgType switch
        {
            OrgType.C => "Corporaration (Company)",
            OrgType.G => "Government Institution",
            OrgType.S => "Social Institution",
            _ => orgType.ToString(),
        };
    }

    public async Task Initialize()
    {
        userData = await _secureRepository.GetAsync<UserData>("userdata");
        if (userData?.Company is null) return;

        var country = new Models.Country
        {
            Name = userData.Company.Country.Name,
            Alpha3 = userData.Company.Country.Alpha3,
            Region = userData.Company.Country.Region
        };

        CompanyCodenameForm.CompanyCountry = country;
        CompanyCodenameForm.OrgType = userData.Company.OrgType;
        CompanyCodenameForm.Title.Value = userData.Company.Title;
        CompanyCodenameForm.VatNumber.Value = userData.Company.VatNumber;
        CompanyCodenameForm.IncorporationDate.Value = userData.Company.IncorporationDate.ToString("yyyy-MM-dd");
        CompanyCodenameForm.Combination.Value = userData.Company.Combination;
    }
}
