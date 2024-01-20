using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Reflection;
using System.Text;
namespace BolWallet.ViewModels;
public partial class CertifyViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IBolService _bolService;
    private readonly IFileDownloadService _fileDownloadService;

    public CertifyViewModel(
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
    private string _certifierCodename = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private int _certifications = 0;

    [ObservableProperty]
    private bool _isCertified = false;

    [ObservableProperty]
    private BolAccount _bolAccount = new();

    [ObservableProperty]
    public Dictionary<string, string> _mandatoryCertifiers;

    public async Task Initialize()
    {
        userData = await _secureRepository.GetAsync<UserData>("userdata");

        await UpdateBolAccount();
    }

    [RelayCommand]
    private async Task UpdateBolAccount()
    {
        try
        {
            BolAccount = await _bolService.GetAccount(userData.Codename);

            if (BolAccount.AccountStatus == AccountStatus.PendingFees)
            {
                IsCertified = true;
                return;
            }

            if (BolAccount.AccountStatus == AccountStatus.PendingCertifications)
            {
                Certifications = BolAccount.Certifications + 1;
                MandatoryCertifiers = BolAccount.MandatoryCertifiers;
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task SelectMandatoryCertifiers()
    {
        try
        {
            IsLoading = true;

            await Task.Delay(100);

            BolAccount bolAccount = await _bolService.SelectMandatoryCertifiers();

            await Toast.Make("Certifiers selected for this certification round").Show();
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
    private async Task RequestCertification()
    {
        try
        {
            if (string.IsNullOrEmpty(CertifierCodename))
                throw new Exception("Please Select Certifier");

            IsLoading = true;

            await Task.Delay(100);

            BolAccount bolAccount = await _bolService.RequestCertification(CertifierCodename);

            CertifierCodename = string.Empty;

            await Toast.Make("Certification request sent successfully.").Show();

            IsLoading = false;
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
    private async Task PayCertificationFees()
    {
        try
        {
            IsLoading = true;

            await Task.Delay(100);

            BolAccount bolAccount = await _bolService.PayCertificationFees();

            await Task.Run(async () => await _secureRepository.SetAsync("userdata", userData));

            IsLoading = false;

            await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
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
    private async Task DownloadEdiFilesAsync(CancellationToken cancellationToken = default)
    {
        var userdata = await this._secureRepository.GetAsync<UserData>("userdata");

        if (string.IsNullOrEmpty(userdata?.EncryptedDigitalMatrix) &&
            string.IsNullOrEmpty(userdata?.EncryptedDigitalMatrixCompany))
            return;

        List<FileItem> files;

        if (userdata.IsIndividualRegistration)
            files = _fileDownloadService.CollectIndividualFilesForDownload(userdata);
        else
            files = _fileDownloadService.CollectCompanyFilesForDownload(userdata);

        var ediZipFiles = await _fileDownloadService.CreateZipFileAsync(files);

        await _fileDownloadService.SaveZipFileAsync(ediZipFiles, cancellationToken);
    }

    [RelayCommand]
    private async Task DownloadAccountAsync(CancellationToken cancellationToken = default)
    {
        await _fileDownloadService.DownloadDataAsync(BolAccount, "BolAccount.json", cancellationToken);
    }

    [RelayCommand]
    private async Task DownloadBolWalletAsync(CancellationToken cancellationToken = default)
    {
        await _fileDownloadService.DownloadDataAsync(userData.BolWallet, "BolWallet.json", cancellationToken);
    }
}
