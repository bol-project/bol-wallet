using System.ComponentModel.DataAnnotations;
using Bol.Core.Abstractions;
using Bol.Core.Rpc.Model;
using BolWallet.Helpers;
using CommunityToolkit.Maui.Alerts;

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
}

public partial class AddMultiCitizenshipViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
    private readonly IBolService _bolService;
    private readonly INinHelper _ninHelper;
    
    public AddMultiCitizenshipViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        IBolService bolService,
        INinHelper ninHelper) : base(navigationService)
    {
        _codeNameService = codeNameService;
        _bolService = bolService;
        _ninHelper = ninHelper;
    }

    public MultiCitizenshipModel MultiCitizenshipModel { get; } = new MultiCitizenshipModel();
    
    [ObservableProperty] private string _shortHash;
    [ObservableProperty] private bool _isMultiCitizenshipRegistered;
    [ObservableProperty] private bool _isLoading;

    public async Task Generate()
    {
        ShortHash = _codeNameService.GenerateShortHash(MultiCitizenshipModel.FirstName, MultiCitizenshipModel.BirthDate.Value, MultiCitizenshipModel.Nin);
        try
        {
            IsMultiCitizenshipRegistered = await _bolService.IsMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);
        }
        catch (RpcException)
        {
            IsMultiCitizenshipRegistered = false;
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
    
    public void ValidateNin()
    {
        var countryCode = MultiCitizenshipModel.CountryCode;
        var nin = MultiCitizenshipModel.Nin;

        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(nin)) return;
        
        (bool _, string errorMessage) = _ninHelper.ValidateNin(nin, countryCode);

        NinValidationErrorMessage = errorMessage;
    }

    public string NinValidationErrorMessage { get; set; }
    public string NinInternationalName => _ninHelper.GetNinInternationalName(MultiCitizenshipModel.CountryCode);
}
