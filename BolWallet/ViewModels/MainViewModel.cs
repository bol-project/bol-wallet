using Bol.Core.Abstractions;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IWalletService _walletService;

    public MainViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IWalletService walletService)
        : base(navigationService)
    {
        _secureRepository = secureRepository;
        _walletService = walletService;
    }

    [ObservableProperty]
    private bool _isLoading = false;

	[RelayCommand]
	private async Task NavigateToCodenameCompanyPage()
	{
        await NavigationService.NavigateTo<CreateCodenameCompanyViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToCodenameIndividualPage()
    {
        await App.Current.MainPage.Navigation.PushAsync(new Views.CitizenshipPage());
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
