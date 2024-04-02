using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels;

public partial class CreateCodenameCompanyViewModel : CreateCodenameViewModel
{
    public CreateCodenameCompanyViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository,
        IBolRpcService bolRpcService,
        IBolService bolService,
        ILogger<CreateCodenameCompanyViewModel> logger) : base(
            navigationService,
            codeNameService,
            secureRepository,
            bolRpcService,
            bolService,
            logger)
    {
        CompanyCodenameForm = new CompanyCodenameForm(content);
    }

    [ObservableProperty]
    private CompanyCodenameForm _companyCodenameForm;

    [RelayCommand]
    private async Task Generate(CancellationToken token = default)
    {
        try
        {
            if (!CompanyCodenameForm.IsFormFilled)
            {
                return;
            }

            userData = await this._secureRepository.GetAsync<UserData>("userdata") ?? new UserData();

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
                IncorporationDate = GetBirthDate(CompanyCodenameForm.IncorporationDate.Value),
                Combination = CompanyCodenameForm.Combination.Value
            };

            var result = _codeNameService.Generate(company);
            var codenameExistsResult = await CodenameExists(result, token);
            
            switch (codenameExistsResult.IsSuccess)
            {
                case true when !codenameExistsResult.Data.Exists:
                    {
                        userData.Codename = result;
                        userData.Company = company;
                        userData.IsIndividualRegistration = false;

                        await _secureRepository.SetAsync("userdata", userData);

                        Codename = result;
                        return;
                    }
                case true when codenameExistsResult.Data.Exists:
                    {
                        var alternatives = string.Join(Environment.NewLine, codenameExistsResult.Data.Alternatives);
                
                        await Toast
                            .Make($"The codename already exists. Found:{Environment.NewLine}{alternatives}",
                                ToastDuration.Long)
                            .Show(token);
                
                        return;
                    }
                case false:
                    {
                        await Toast.Make($"Codename existence check failed with error: {codenameExistsResult.Message}",
                            ToastDuration.Long).Show(token);
                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(token);
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
