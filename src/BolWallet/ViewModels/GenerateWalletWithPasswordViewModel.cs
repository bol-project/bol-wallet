using Bol.Core.Abstractions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet.ViewModels;

public partial class GenerateWalletWithPasswordViewModel : BaseViewModel
{
    private readonly IWalletService _walletService;
    private readonly ISecureRepository _secureRepository;
    private readonly IFileDownloadService _fileDownloadService;
    private readonly IDeviceDisplay _deviceDisplay;
    private readonly IMessenger _messenger;

    public GenerateWalletWithPasswordViewModel(
        INavigationService navigationService,
        IWalletService walletService,
        ISecureRepository secureRepository,
        IFileDownloadService fileDownloadService,
        IDeviceDisplay deviceDisplay,
        IMessenger messenger)
        : base(navigationService)
    {
        _walletService = walletService;
        _secureRepository = secureRepository;
        _fileDownloadService = fileDownloadService;
        _deviceDisplay = deviceDisplay;
        _messenger = messenger;
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
                await NavigationService.NavigateTo<DownloadCertificationDocumentsViewModel>();
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

            _messenger.Send(Constants.WalletCreatedMessage);
            
            await NavigationService.NavigateTo<DownloadCertificationDocumentsViewModel>();
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
