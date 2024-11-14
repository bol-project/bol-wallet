using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels;

public partial class PreloadViewModel : BaseViewModel
{
    private readonly IBolRpcService _bolRpcService;
    private readonly INetworkPreferences _networkPreferences;
    private readonly ICountriesService _countriesService;
    private readonly ISecureRepository _secureRepository;
    private readonly ILogger _logger;

    public PreloadViewModel(
        INavigationService navigationService,
        IBolRpcService bolRpcService,
        INetworkPreferences networkPreferences,
        ICountriesService countriesService,
        ISecureRepository secureRepository,
        ILogger<PreloadViewModel> logger) : base(navigationService)
    {
        _bolRpcService = bolRpcService;
        _networkPreferences = networkPreferences;
        _countriesService = countriesService;
        _secureRepository = secureRepository;
        _logger = logger;
    }
    
    public override async Task OnInitializedAsync()
    {
        try
        {
            await LoadAndNavigate();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user and navigate to main viewmodel during preload...");
            throw;
        }

        async Task LoadAndNavigate()
        {
            var result = await _bolRpcService.GetBolContractHash(CancellationToken.None);
            if (result.IsFailed)
            {
                await Toast.Make(result.Message).Show(CancellationToken.None);
                return;
            }

            _networkPreferences.SetBolContractHash(result.Data);
        
            _ = await _countriesService.GetAsync();
        
            UserData userData = _secureRepository.Get<UserData>("userdata");

            if (userData?.BolWallet == null)
            {
                await NavigationService.NavigateTo<MainViewModel>();
            }
            else
            {
                await NavigationService.NavigateTo<MainWithAccountViewModel>();
            }
        }
    }
}
