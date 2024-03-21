using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace BolWallet.ViewModels;

public partial class AccountViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IBolService _bolService;
    private readonly IFileDownloadService _fileDownloadService;

    public AccountViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IBolService bolService,
        IFileDownloadService fileDownloadService) : base(navigationService)
    {
        _secureRepository = secureRepository;
        _bolService = bolService;
        _fileDownloadService = fileDownloadService;
    }

    [ObservableProperty]
    private BolAccount _bolAccount;

    [ObservableProperty]
    private List<KeyValuePair<string, string>> _certifiers;

    [ObservableProperty]
    private List<string> _certificationRequests;

    public async Task Initialize(CancellationToken cancellationToken = default)
    {
        try
        {
            userData = await _secureRepository.GetAsync<UserData>("userdata");

            BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));

            Certifiers = BolAccount.Certifiers.ToList();
            CertificationRequests = BolAccount.CertificationRequests.Keys.ToList();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(cancellationToken);
        }
    }

    [RelayCommand]
    private async Task DownloadEdiFilesAsync(CancellationToken cancellationToken = default)
    {
        var userdata = await this._secureRepository.GetAsync<UserData>("userdata");

        if (string.IsNullOrEmpty(userdata?.EncryptedDigitalMatrix) &&
            string.IsNullOrEmpty(userdata?.EncryptedDigitalMatrixCompany))
        {
            await Toast.Make("Encrypted Digital Matrix not found in the device.").Show(cancellationToken);
            return;
        }

        List<FileItem> files;

        if (userdata.IsIndividualRegistration)
            files = _fileDownloadService.CollectIndividualFilesForDownload(userdata);
        else
            files = _fileDownloadService.CollectCompanyFilesForDownload(userdata);

        var ediZipFiles = await _fileDownloadService.CreateZipFileAsync(files);

        await _fileDownloadService.SaveZipFileAsync(userdata.Codename, ediZipFiles, cancellationToken);
    }

    [RelayCommand]
    private async Task DownloadAccountAsync(CancellationToken cancellationToken = default)
    {
        await _fileDownloadService.DownloadDataAsync(BolAccount, $"BolAccount_{BolAccount.CodeName}.json", cancellationToken);
    }

    [RelayCommand]
    private async Task DownloadBolWalletAsync(CancellationToken cancellationToken = default)
    {
        await _fileDownloadService.DownloadDataAsync(userData.BolWallet, $"BolWallet_{BolAccount.CodeName}.json", cancellationToken);
    }
}
