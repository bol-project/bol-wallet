using Bol.Core.Abstractions;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using System.Text;

namespace BolWallet.ViewModels;

public partial class GenerateWalletWithPasswordViewModel : BaseViewModel
{
    private readonly IWalletService _walletService;
    private readonly ISecureRepository _secureRepository;
    private readonly IFileDownloadService _fileDownloadService;
    private readonly IDeviceDisplay _deviceDisplay;

    public GenerateWalletWithPasswordViewModel(
        INavigationService navigationService,
        IWalletService walletService,
        ISecureRepository secureRepository,
        IFileDownloadService fileDownloadService,
        IDeviceDisplay deviceDisplay)
        : base(navigationService)
    {
        _walletService = walletService;
        _secureRepository = secureRepository;
        _fileDownloadService = fileDownloadService;
        _deviceDisplay = deviceDisplay;
    }

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _walletCreationProgress = "Please keep the application open...";

    public async Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            userData = await this._secureRepository.GetAsync<UserData>("userdata");

            if (userData.BolWallet is not null)
                await App.Current.MainPage.Navigation.PushAsync(new Views.DownloadCertificationDocumentsPage());
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(cancellationToken);
        }
    }

    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            if (userData.BolWallet is not null)
            {
                await App.Current.MainPage.Navigation.PushAsync(new Views.DownloadCertificationDocumentsPage());
                return;
            }

            _deviceDisplay.KeepScreenOn = true;
            IsLoading = true;

            Bol.Core.Model.BolWallet bolWallet;

            if (userData.IsIndividualRegistration)
                bolWallet = await Task.Run(() => _walletService.CreateWalletB(Password, userData.Codename, userData.Edi));
            else
                bolWallet = await Task.Run(() => _walletService.CreateWalletC(Password, userData.Codename, userData.Edi));

            userData.BolWallet = bolWallet;
            userData.WalletPassword = Password;

            await Task.Run(async () => await _secureRepository.SetAsync("userdata", userData));

            await DownloadWalletAsync(bolWallet);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            _deviceDisplay.KeepScreenOn = false;
            IsLoading = false;
            GC.Collect();
        }
    }

    [RelayCommand]
    private async Task DownloadWalletAsync(
        Bol.Core.Model.BolWallet bolWallet,
        CancellationToken cancellationToken = default)
    {
        await _fileDownloadService.DownloadDataAsync(bolWallet, "BolWallet.json", cancellationToken);
    }
}
