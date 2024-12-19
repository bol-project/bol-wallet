using System.ComponentModel.DataAnnotations;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Rpc.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class CertifyViewModel : ObservableValidator
{
    private readonly IBolService _bolService;
    private readonly INavigationService _navigation;

    public CertifyViewModel(IBolService bolService, INavigationService navigation)
    {
        _bolService = bolService;
        _navigation = navigation;

        _selectedExtraCitizenships = new List<string> { "", "" };
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [Required]
    private string _codeName;

    [ObservableProperty]
    private int _citizenshipCount = 1;

    [ObservableProperty]
    private List<BolAccount> _alternativeAccounts = new();

    [ObservableProperty]
    private bool _isMultiCitizenship;

    [ObservableProperty]
    private List<string> _selectedExtraCitizenships;

    [ObservableProperty]
    private bool _isAlternativeCertified;

    [ObservableProperty]
    private bool _certifyDisabled;

    [ObservableProperty]
    private bool _isCheckProcessOver;

    public async Task Lookup()
    {
        try
        {
            IsCheckProcessOver = false;
            IsLoading = true;
            var alternativeCodeNames = (await _bolService.FindAlternativeCodeNames(CodeName)).ToArray();
            var alternativeAccounts = new List<BolAccount>(alternativeCodeNames.Length);
            var codeNameParts = CodeName.Split("<");
            var countryCode = codeNameParts[1];
            var shortHash = codeNameParts[7];

            SelectedExtraCitizenships.Add(countryCode);

            var primaryAccount = await _bolService.GetAccount(CodeName);

            foreach (var citizenship in SelectedExtraCitizenships)
            {
                if (string.IsNullOrEmpty(citizenship)) continue;

                try
                {
                    var isMulti = await _bolService.IsMultiCitizenship(citizenship, shortHash);

                    if (isMulti && primaryAccount.Certifications == 0)
                    {
                        IsMultiCitizenship = true;
                        break;
                    }
                    else
                    {
                        IsMultiCitizenship = false;
                    }
                }
                catch (RpcException ex)
                {
                    IsMultiCitizenship = false;
                }
            }

            foreach (var codeName in alternativeCodeNames)
            {
                var account = await _bolService.GetAccount(codeName);
                alternativeAccounts.Add(account);
            }
            AlternativeAccounts = alternativeAccounts;

            IsAlternativeCertified = (AlternativeAccounts.Count > 1 &&
                                      AlternativeAccounts
                                          .Where(account => account.CodeName != CodeName)
                                          .Any(account => account.Certifications > 1));

            CertifyDisabled = IsMultiCitizenship || IsAlternativeCertified;
            IsCheckProcessOver = true;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task Certify()
    {
        try
        {
            if (string.IsNullOrEmpty(CodeName))
                throw new Exception("Please Select a CodeName");

            IsLoading = true;


            var codeNameParts = CodeName.Split("<");
            var countryCode = codeNameParts[1];
            var shortHash = codeNameParts[7];

            SelectedExtraCitizenships.Remove(countryCode);

            foreach (var citizenship in SelectedExtraCitizenships)
            {
                if (string.IsNullOrEmpty(citizenship)) continue;

                bool isMultiCitizenshipRegistered = false;
                try
                {
                    isMultiCitizenshipRegistered = await _bolService.AddMultiCitizenship(citizenship, shortHash);
                }
                catch (RpcException)
                {
                    isMultiCitizenshipRegistered = false;
                }
            }

            await _bolService.Certify(CodeName);

            await Toast.Make($"{CodeName} has received the certification.").Show();

            await _navigation.NavigateTo<MainWithAccountViewModel>();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
