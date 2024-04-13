using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Rpc.Model;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;

namespace BolWallet.ViewModels;
public partial class MainWithAccountViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IBolService _bolService;
    private readonly IDeviceDisplay _deviceDisplay;
    private readonly IAddressTransformer _addressTransformer;
    private readonly IBolRpcService _bolRpcClient;
    private readonly IOptions<BolWalletAppConfig> _bolConfig;
    private readonly IOptions<BolConfig> _bolSdkConfig;

    public string WelcomeText => "Welcome";
    public string BalanceText => "Total Balance";
    public string AccountText => "Account";
    public string SendText => "Transfer";
    public string CommunityText => "Bol Community";

    [ObservableProperty]
    private List<BalanceDisplayItem> _commercialBalancesDisplayList = new();

    [ObservableProperty]
    private string _codeName = "";

    [ObservableProperty]
    private string _mainAddress = "";

    [ObservableProperty]
    private BolAccount _bolAccount = new();

    [ObservableProperty]
    private bool _isRefreshing = false;
    
    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _isAccountOpen = false;

    [ObservableProperty]
    private bool _isRegistered = false;
    
    [ObservableProperty]
    private bool _isWhiteListed = false;

    [ObservableProperty]
    private bool _canRegister = false;
    
    [ObservableProperty]
    private bool _canWhiteList = false;

    [ObservableProperty]
    private bool _isCommercialAddressesExpanded = false;

    public MainWithAccountViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IBolService bolService,
        IDeviceDisplay deviceDisplay, 
        IAddressTransformer addressTransformer,
        IBolRpcService bolRpcClient,
        IOptions<BolWalletAppConfig> bolConfig,
        IOptions<BolConfig> bolSdkConfig)
        : base(navigationService)
    {
        _secureRepository = secureRepository;
        _bolService = bolService;
        _deviceDisplay = deviceDisplay;
        _addressTransformer = addressTransformer;
        _bolRpcClient = bolRpcClient;
        _bolConfig = bolConfig;
        _bolSdkConfig = bolSdkConfig;
    }

    [RelayCommand]
    private async Task Refresh(CancellationToken token)
    {
        try
        {
            _deviceDisplay.KeepScreenOn = true;
            IsLoading = true;

            if (await TrySetBolContractHash(token))
            {
                await FetchBolAccountData(token);
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(token);
        }
        finally
        {
            _deviceDisplay.KeepScreenOn = false;
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    private async Task<bool> TrySetBolContractHash(CancellationToken token)
    {
        if (!string.IsNullOrWhiteSpace(_bolSdkConfig.Value.Contract))
        {
            return true;
        }

        var result = await _bolRpcClient.GetBolContractHash(token);
        if (result.IsFailed)
        {
            await Toast.Make(result.Message).Show(token);
            return false;
        }

        _bolSdkConfig.Value.Contract = result.Data;
        return true;
    }

    private async Task FetchBolAccountData(CancellationToken token)
    {
        try
        {
            userData = await _secureRepository.GetAsync<UserData>("userdata");

            CodeName = userData.Codename;
            MainAddress = userData?.BolWallet?.accounts?.FirstOrDefault(a => a.Label == "main")?.Address
                ?? throw new Exception("Could not find a Main Address account in open wallet.");

            try
            {
                BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename, token), token);
                IsRegistered = true;
            }
            catch (RpcException ex)
            {
                IsRegistered = false;
                await Toast.Make(ex.Message).Show();
            }

            if (IsRegistered)
            {
                userData.IsCertifier = BolAccount.IsCertifier;
                await _secureRepository.SetAsync("userdata", userData);

                if (BolAccount.AccountStatus == AccountStatus.Open)
                    IsAccountOpen = true;
                else
                    await NavigationService.NavigateTo<GetCertifiedViewModel>(true);

                CommercialBalancesDisplayList = (BolAccount?.CommercialBalances ?? new())
                    .Select(pair => new BalanceDisplayItem { Address = pair.Key, Balance = pair.Value })
                    .ToList();

                await App.Current.MainPage.Navigation.PushAsync(new HomePage());
                return;
            }
            
            try
            {
                IsWhiteListed = await Task.Run(async () => 
                    await _bolService.IsWhitelisted(_addressTransformer.ToScriptHash(MainAddress), token), token);
            }
            catch (RpcException ex)
            {
                IsWhiteListed = false;
                await Toast.Make(ex.Message).Show();
            }

            CanWhiteList = !IsWhiteListed && !IsRegistered;
            CanRegister = IsWhiteListed && !IsRegistered;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task Register(CancellationToken token)
    {
        try
        {
            IsLoading = true;

            await _bolService.Register(token);

            while (!IsRegistered)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                await FetchBolAccountData(token);
            }
            await Toast.Make("Your Account has been registered.").Show();
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

    [RelayCommand]
    private async Task Whitelist(CancellationToken token)
    {
        try
        {
            IsLoading = true;

            Uri uri = new Uri($"{_bolConfig.Value.BolCertifierEndpoint}/{MainAddress}");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            
            while (!IsWhiteListed)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                await FetchBolAccountData(token);
            }
            await Toast.Make("Your Main Address has been Whitelisted.").Show();
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

    [RelayCommand]
    private void ExpandCommercialAddress()
    {
        IsCommercialAddressesExpanded = !IsCommercialAddressesExpanded;
    }

    [RelayCommand]
    private async Task NavigateToTransactionsPage()
    {
        await NavigationService.NavigateTo<TransactionsViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToBolCommunityPage()
    {
        await NavigationService.NavigateTo<BolCommunityViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToFinancialTransactionsPage()
    {
        await NavigationService.NavigateTo<FinancialTransactionsViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToCertifierPage()
    {
        await NavigationService.NavigateTo<BolCommunityViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToAccountPage()
    {
        await NavigationService.NavigateTo<AccountViewModel>(true);
    }
}


