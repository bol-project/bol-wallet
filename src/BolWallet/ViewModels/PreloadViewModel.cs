using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class PreloadViewModel : BaseViewModel
{
    private readonly IBolRpcService _bolRpcService;
    private readonly INetworkPreferences _networkPreferences;
    private readonly ICountriesService _countriesService;
    private readonly ISecureRepository _secureRepository;

    public PreloadViewModel(
        INavigationService navigationService,
        IBolRpcService bolRpcService,
        INetworkPreferences networkPreferences,
        ICountriesService countriesService,
        ISecureRepository secureRepository) : base(navigationService)
    {
        _bolRpcService = bolRpcService;
        _networkPreferences = networkPreferences;
        _countriesService = countriesService;
        _secureRepository = secureRepository;
    }
    
    public override async Task OnInitializedAsync()
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
            await NavigationService.NavigateTo<MainViewModel>(changeRoot: true);
        }
        else
        {
            await NavigationService.NavigateTo<MainWithAccountViewModel>(changeRoot: true);
        }
    }
}
