using Bol.Core.Abstractions;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IWalletService _walletService;
    private readonly INetworkPreferences _networkPreferences;
    
    public MainViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IWalletService walletService,
        INetworkPreferences networkPreferences)
        : base(navigationService)
    {
        _secureRepository = secureRepository;
        _walletService = walletService;
        _networkPreferences = networkPreferences;
        
        WelcomeMessage = $"Welcome to Bol! ({_networkPreferences.Name})";
        SwitchToNetworkText = $"Switch to {_networkPreferences.AlternativeName}";
    }

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _welcomeMessage;
    
    [ObservableProperty]
    private string _switchToNetworkText;

    [RelayCommand]
    private void SwitchNetwork()
    {
        _networkPreferences.SwitchNetwork();
        WelcomeMessage = $"Welcome to Bol! ({_networkPreferences.Name})";
        SwitchToNetworkText = $"Switch to {_networkPreferences.AlternativeName}";
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
