using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Bol.Core.Abstractions;
using Bol.Core.Rpc.Model;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels;

public class MultiCitizenshipModel
{
    [Required]
    public string CountryCode { get; set; }

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string FirstName { get; set; }
        
    [Required]
    public string Nin { get; set; }

    [Required] 
    public DateTime? BirthDate { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Short Hash must be exactly 10 or 11 characters.")]
    public string ShortHash { get; set; }
}

public class MultiCitizenshipShortHashModel
{
    [Required]
    public string CountryCode { get; set; }

    [Required]
    [StringLength(11, MinimumLength = 10, ErrorMessage = "Short Hash must be exactly 10 or 11 characters.")]
    public string ShortHash { get; set; }
}


public partial class AddMultiCitizenshipViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
    private readonly IBolService _bolService;
    private readonly RegisterContent _registerContent;
    private readonly ILogger<AddMultiCitizenshipViewModel> _logger;

    public AddMultiCitizenshipViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        IBolService bolService,
        RegisterContent registerContent,
        ILogger<AddMultiCitizenshipViewModel> logger) : base(navigationService)
    {
        _codeNameService = codeNameService;
        _bolService = bolService;
        _registerContent = registerContent;
        _logger = logger;
    }

    public MultiCitizenshipModel MultiCitizenshipModel { get; set; } = new MultiCitizenshipModel();
    public MultiCitizenshipShortHashModel MultiCitizenshipShortHashModel { get; set; } = new MultiCitizenshipShortHashModel();

    [ObservableProperty] private string _shortHash;
    [ObservableProperty] private bool _isMultiCitizenshipRegistered;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isKnownShortHash;

    public async Task Generate()
    {
        ShortHash = _codeNameService.GenerateShortHash(MultiCitizenshipModel.FirstName, MultiCitizenshipModel.BirthDate.Value, MultiCitizenshipModel.Nin);
        try
        {
            IsMultiCitizenshipRegistered = await _bolService.IsMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);
        }
        catch (RpcException ex)
        {
            IsMultiCitizenshipRegistered = false;

            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task CheckMultiCitizenship()
    {
        try
        {
            ShortHash = MultiCitizenshipShortHashModel.ShortHash;
            IsMultiCitizenshipRegistered = await _bolService.IsMultiCitizenship(MultiCitizenshipShortHashModel.CountryCode, ShortHash);
        }
        catch (RpcException ex)
        {
            IsMultiCitizenshipRegistered = false;

            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task Register()
    {
        try
        {
            IsLoading = true;
            await _bolService.AddMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);

            for (int i = 0; i < 20; i++)
            {
                try
                {
                    IsMultiCitizenshipRegistered =
                        await _bolService.IsMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);
                }
                catch (RpcException)
                {
                    IsMultiCitizenshipRegistered = false;
                }

                if (IsMultiCitizenshipRegistered) break;
                await Task.Delay(1000);
            }
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

    public void OnKnownShortHashToggleChange(bool newValue)
    {
        IsKnownShortHash = newValue;
        MultiCitizenshipModel = new MultiCitizenshipModel();
        MultiCitizenshipShortHashModel = new MultiCitizenshipShortHashModel();
        ShortHash = string.Empty;
    }

    public void ValidateNin()
    {
        var countryCode = MultiCitizenshipModel.CountryCode;
        var nin = MultiCitizenshipModel.Nin;

        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(nin)) return;

        string Pattern = _registerContent.NinPerCountryCode[countryCode].Regex;
        var regex = new Regex(Pattern);

        var ninRequiredDigits = _registerContent.NinPerCountryCode[countryCode].Digits;

        bool isNinValid = regex.IsMatch(nin);
        bool isNinLengthCorrect = nin.Length == 5;

        if (isNinValid && isNinLengthCorrect)
        {
            NinValidationErrorMessage = "";
            return;
        }

        NinValidationErrorMessage =
               $"The National Identification Number (NIN) provided does not match the expected length of 5 digits." +
               " Please ensure that only capital letters (A-Z) and numbers are used in the NIN.";
    }

    public string NinValidationErrorMessage { get; set; }
    public string NinInternationalName => string.IsNullOrWhiteSpace(MultiCitizenshipModel.CountryCode) 
        ? ""
        : _registerContent.NinPerCountryCode[MultiCitizenshipModel.CountryCode]?.InternationalName;
}
