using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Rpc.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class CertifyViewModel : ObservableValidator
{
    private readonly IBolService _bolService;
    private readonly INavigationService _navigation;
    private readonly ICodeNameService _codeNameService;
    private readonly RegisterContent _registerContent;

    public CertifyViewModel(
        IBolService bolService, 
        INavigationService navigation,
        ICodeNameService codeNameService,
        RegisterContent registerContent)
    {
        _bolService = bolService;
        _navigation = navigation;
        _codeNameService = codeNameService;
        _registerContent = registerContent;
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [Required]
    private string _codeName;

    [ObservableProperty]
    [Required(ErrorMessage = "Please select the number of citizenship(s).")]
    [Range(1, 3, ErrorMessage = "You must select between 1 and 3 citizenships.")]
    private int _citizenshipCount = 0;

    [ObservableProperty]
    private List<BolAccount> _alternativeAccounts = new();

    [ObservableProperty]
    private bool _isMultiCitizenship;

    [ObservableProperty]
    private bool _isAlternativeCertified;

    [ObservableProperty]
    private bool _certifyDisabled;

    [ObservableProperty]
    private bool _isCheckProcessOver;

    [ObservableProperty]
    private bool _isCodenameShorthashRegistered;

    [ObservableProperty]
    private string _codenameCountryCode;

    [ObservableProperty]
    private List<MultiCitizenshipModel> _multiCitizenships = new List<MultiCitizenshipModel>();

    public async Task Lookup()
    {
        if (!ValidateForm())
        {
            await Toast.Make("Please correct the errors in the form before submitting.").Show();
            return;
        }

        try
        {
            var codeNameParts = CodeName.Split("<");
            var countryCode = codeNameParts[1];
            CodenameCountryCode = countryCode;
            var shortHash = codeNameParts[7];

            if (MultiCitizenships.Select(m => m.CountryCode).Contains(countryCode))
            {
                await Toast.Make("The selected country matches the primary country in the CodeName. Please choose a different country for additional citizenship.").Show();
                return;
            }

            IsCheckProcessOver = false;
            IsLoading = true;
            var alternativeCodeNames = (await _bolService.FindAlternativeCodeNames(CodeName)).ToArray();
            var alternativeAccounts = new List<BolAccount>(alternativeCodeNames.Length);

            var primaryAccount = await _bolService.GetAccount(CodeName);

            foreach (var citizenship in MultiCitizenships)
            {
                if (string.IsNullOrEmpty(citizenship.CountryCode)) continue;

                try
                {
                    var generatedShortHash = _codeNameService.GenerateShortHash(citizenship.FirstName, citizenship.BirthDate.Value, citizenship.Nin);

                    var isMulti = await _bolService.IsMultiCitizenship(citizenship.CountryCode, generatedShortHash);

                    if (isMulti)
                    {
                        citizenship.IsInMultiCitizenshipList = true;
                    }

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

            try
            {
                IsMultiCitizenship = await _bolService.IsMultiCitizenship(countryCode, shortHash);
                IsCodenameShorthashRegistered = true;
            }
            catch (RpcException ex)
            {
                if (!IsMultiCitizenship)
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

            await _bolService.Certify(CodeName);

            foreach (var citizenship in MultiCitizenships)
            {
                if (string.IsNullOrEmpty(citizenship.CountryCode)) continue;

                bool isMultiCitizenshipRegistered = false;
                try
                {
                    if (!citizenship.IsInMultiCitizenshipList)
                    {
                        var generatedShortHash = _codeNameService.GenerateShortHash(citizenship.FirstName, citizenship.BirthDate.Value, citizenship.Nin);

                        isMultiCitizenshipRegistered = await _bolService.AddMultiCitizenship(citizenship.CountryCode, generatedShortHash);
                    }
                }
                catch (RpcException)
                {
                    isMultiCitizenshipRegistered = false;
                }
            }

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

    partial void OnCitizenshipCountChanged(int value)
    {
        UpdateMultiCitizenshipCount(value);
    }

    public void UpdateMultiCitizenshipCount(int count)
    {
        if (count == 1)
        {
            MultiCitizenships.Clear();
        }
        else
        {
            while (MultiCitizenships.Count < count - 1)
            {
                MultiCitizenships.Add(new MultiCitizenshipModel());
            }

            while (MultiCitizenships.Count > count - 1)
            {
                MultiCitizenships.RemoveAt(MultiCitizenships.Count - 1);
            }
        }
    }

    public void ValidateNin()
    {
        foreach (var citizenship in MultiCitizenships)
        {
            var countryCode = citizenship.CountryCode;
            var nin = citizenship.Nin;

            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(nin))
            {
                citizenship.NinValidationErrorMessage = "Country code and NIN are required.";
                continue;
            }

            if (!_registerContent.NinPerCountryCode.ContainsKey(countryCode))
            {
                citizenship.NinValidationErrorMessage = $"Invalid country code: {countryCode}.";
                continue;
            }

            var pattern = _registerContent.NinPerCountryCode[countryCode].Regex;
            var regex = new Regex(pattern);

            var ninRequiredDigits = _registerContent.NinPerCountryCode[countryCode].Digits;

            bool isNinValid = regex.IsMatch(nin);
            bool isNinLengthCorrect = nin.Length == 5;

            if (isNinValid && isNinLengthCorrect)
            {
                citizenship.NinValidationErrorMessage = ""; // Valid NIN
            }
            else
            {
                citizenship.NinValidationErrorMessage =
                    $"The National Identification Number (NIN) provided for country {countryCode} does not match the expected length of 5 digits." +
                    " Please ensure that only capital letters (A-Z) and numbers are used in the NIN.";
            }
        }
    }

    public bool ValidateForm()
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this);
        var isValid = Validator.TryValidateObject(this, context, validationResults, true);

        foreach (var citizenship in MultiCitizenships)
        {
            var citizenshipContext = new ValidationContext(citizenship);
            isValid &= Validator.TryValidateObject(citizenship, citizenshipContext, validationResults, true);
        }

        return isValid;
    }

}
