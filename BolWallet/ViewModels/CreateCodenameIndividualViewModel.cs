using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels;

public partial class CreateCodenameIndividualViewModel : CreateCodenameViewModel
{
    public CreateCodenameIndividualViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository,
        IBolRpcService bolRpcService,
        IBolService bolService,
        ILogger<CreateCodenameViewModel> logger) : base(
            navigationService,
            codeNameService,
            secureRepository,
            bolRpcService,
            bolService,
            logger)
    {
        IndividualCodenameForm = new IndividualCodenameForm(content);
    }

    [ObservableProperty]
    private IndividualCodenameForm _individualCodenameForm;

    [RelayCommand]
    private async Task Generate(CancellationToken token = default)
    {
        try
        {
            if (!IndividualCodenameForm.IsFormFilled)
            {
                return;
            }

            userData = await this._secureRepository.GetAsync<UserData>("userdata");

            var person = new NaturalPerson
            {
                FirstName = IndividualCodenameForm.FirstName.Value,
                MiddleName = IndividualCodenameForm.MiddleName.Value,
                Surname = IndividualCodenameForm.Surname.Value,
                ThirdName = IndividualCodenameForm.ThirdName.Value,
                Gender = IndividualCodenameForm.Gender,
                Combination = IndividualCodenameForm.Combination.Value,
                Nin = IndividualCodenameForm.NIN.Value,
                Birthdate = GetBirthDate(IndividualCodenameForm.Birthdate.Value),
                CountryCode = IndividualCodenameForm.SelectedCountry.Alpha3
            };

            userData.BirthCountryCode = IndividualCodenameForm.CountryOfBirth.Alpha3;

            var result = _codeNameService.Generate(person);
            var codenameExistsResult = await CodenameExists(result, token);
            
            switch (codenameExistsResult.IsSuccess)
            {
                case true when !codenameExistsResult.Data.Exists:
                    {
                        userData.Codename = result;
                        userData.Person = person;
                        userData.IsIndividualRegistration = true;

                        await _secureRepository.SetAsync("userdata", userData);

                        Codename = result;
                        return;
                    }
                case true when codenameExistsResult.Data.Exists:
                    {
                        var alternatives = string.Join(Environment.NewLine, codenameExistsResult.Data.Alternatives);
                
                        await Toast
                            .Make($"The codename already exists. Please consider some of the {alternatives}",
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

    public async Task Initialize()
    {
        userData = await _secureRepository.GetAsync<UserData>("userdata");
        if (userData?.Person is null) return;

        IndividualCodenameForm.Gender = userData.Person.Gender;
        IndividualCodenameForm.Combination.Value = userData.Person.Combination;
        IndividualCodenameForm.Birthdate.Value = userData.Person.Birthdate.ToString("yyyy-MM-dd");
        IndividualCodenameForm.CountryOfBirth = IndividualCodenameForm
                    .Countries
                    .FirstOrDefault(c => c.Alpha3 == userData.BirthCountryCode);
    }
}

