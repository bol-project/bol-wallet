using Bol.Core.Abstractions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IWalletService _walletService;
    private readonly INetworkPreferences _networkPreferences;
    private readonly IBolRpcService _bolRpcService;

    public MainViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IWalletService walletService,
        INetworkPreferences networkPreferences,
        IBolRpcService bolRpcService)
        : base(navigationService)
    {
        _secureRepository = secureRepository;
        _walletService = walletService;
        _networkPreferences = networkPreferences;
        _bolRpcService = bolRpcService;
        
        WelcomeMessage = $"Welcome to Bol! ({_networkPreferences.Name})";
        SwitchToNetworkText = $"Switch to {_networkPreferences.AlternativeName}";
    }

    public override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(_networkPreferences.TargetNetworkConfig.Contract))
        {
            await TrySetBolContractHash();
        }
        
        await base.OnInitializedAsync();
    }

    private async Task TrySetBolContractHash()
    {
        var result = await _bolRpcService.GetBolContractHash();
        if (result.IsFailed)
        {
            await Toast.Make(result.Message).Show();
            return;
        }

        _networkPreferences.SetBolContractHash(result.Data);
    }

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _welcomeMessage = Constants.WelcomeMessage;
    
    [ObservableProperty]
    private string _switchToNetworkText;
    
    [RelayCommand]
    private async Task SwitchNetwork()
    {
        _networkPreferences.SwitchNetwork();
        WelcomeMessage = _networkPreferences.IsMainNet ? Constants.WelcomeMessage : $"Welcome to Bol! ({_networkPreferences.Name})";
        SwitchToNetworkText = $"Switch to {_networkPreferences.AlternativeName}";
        await TrySetBolContractHash();
    }
    
    [RelayCommand]
	private async Task NavigateToCodenameCompanyPage()
	{
        await NavigationService.NavigateTo<CreateCodenameCompanyViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToWalletCreationInfoPage()
    {
        await App.Current.MainPage.Navigation.PushAsync(new Views.WalletCreationInfo());
    }

    [RelayCommand]
    private async Task ImportYourWallet()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.MacCatalyst, new[] { "json" } },
                { DevicePlatform.WinUI, new[] { ".json", "application/json" } }
            });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType, PickerTitle = "Import Your Json Wallet"
            });

            if (pickResult == null)
                return;

            var jsonString = await File.ReadAllTextAsync(pickResult.FullPath);
            
            var passwordPopup = new PasswordPopup();
            await Application.Current.MainPage.ShowPopupAsync(passwordPopup);
            var password = await passwordPopup.TaskCompletionSource.Task;
            
            if (string.IsNullOrEmpty(password)) return;

            try
            {
                IsLoading = true;
                var validPassword = await Task.Run(() => _walletService.CheckWalletPassword(jsonString, password));
                if (!validPassword)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Incorrect Password",
                        "Please provide a valid password.",
                        "OK");
                    return;
                }
            }
            finally
            { 
                IsLoading = false;
            }
            
            var bolWallet = JsonSerializer.Deserialize<Bol.Core.Model.BolWallet>(jsonString,
                    Constants.WalletJsonSerializerDefaultOptions);

            var userData = new UserData { Codename = bolWallet.Name, BolWallet = bolWallet, WalletPassword = password };

            await _secureRepository.SetAsync("userdata", userData);

            await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }
}
