using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class GetCertifiedViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IBolService _bolService;
    private readonly IFileDownloadService _fileDownloadService;

    public GetCertifiedViewModel(
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
    private int _certificationRound = 0;

    [ObservableProperty]
    private bool _isAccountPendingFees = false;

    [ObservableProperty]
    private bool _isAccountOpen = false;

    [ObservableProperty]
    private bool _certifiersSelected = false;

    [ObservableProperty]
    private bool _canSelectCertifiers = false;

    [ObservableProperty]
    private BolAccount _bolAccount = new();

    [ObservableProperty]
    private List<CertifierListItem> _mandatoryCertifiers = new();

    [ObservableProperty]
    private List<CertifierListItem> _certifiers = new();

    public async Task Initialize(CancellationToken token)
    {
        try
        {
            userData = await _secureRepository.GetAsync<UserData>("userdata");

            IsLoading = true;
            await UpdateBolAccount(token);
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
    private async Task Refresh(CancellationToken token)
    {
        try
        {
            IsLoading = true;
            await UpdateBolAccount(token);
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
    
    private async Task UpdateBolAccount(CancellationToken token)
    {
        try
        {
            BolAccount = await _bolService.GetAccount(userData.Codename, token);
            IsAccountOpen = BolAccount.AccountStatus == AccountStatus.Open;
            CertificationRound = BolAccount.Certifications + 1;
            IsAccountPendingFees = BolAccount.AccountStatus == AccountStatus.PendingFees;

            if (IsAccountPendingFees)
            {
                var certifiers = new List<CertifierListItem>();
                foreach (var codeName in BolAccount.Certifiers.Keys)
                {
                    var certifier = await _bolService.GetAccount(codeName, token);
                    certifiers.Add(new CertifierListItem{
                        CodeName = codeName,
                        Fee = certifier.CertificationFee,
                        Color = Colors.WhiteSmoke
                    });
                }

                Certifiers = certifiers;
            }

            var mandatoryCertifiers = new List<CertifierListItem>();
            if (BolAccount.MandatoryCertifiers.Count != 0)
            {
                CertifiersSelected = true;
                foreach (var codeName in BolAccount.MandatoryCertifiers.Keys)
                {
                    var certifier = await _bolService.GetAccount(codeName, token);
                    mandatoryCertifiers.Add(new CertifierListItem
                    {
                        CodeName = codeName,
                        Fee = certifier.CertificationFee,
                        Color = BolAccount.CertificationRequests.ContainsKey(codeName)
                            ? Colors.LightSeaGreen
                            : Colors.WhiteSmoke
                    });
                }
            }
            else
            {
                CertifiersSelected = false;
            }
            MandatoryCertifiers = mandatoryCertifiers;

            CanSelectCertifiers = !CertifiersSelected && 
                                  !IsAccountPendingFees &&
                                  BolAccount.AccountStatus != AccountStatus.Locked;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task SelectMandatoryCertifiers(CancellationToken token)
    {
        try
        {
            if (MandatoryCertifiers.Count != 0)
            {
                await Toast.Make("Certifiers already selected for this certification round.").Show();
                return;
            }
            
            IsLoading = true;

            await _bolService.SelectMandatoryCertifiers(token);

            while (MandatoryCertifiers.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                await UpdateBolAccount(token);
            }

            await Toast.Make("Certifiers selected for this certification round.").Show();
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
    private async Task RequestCertification(CancellationToken token)
    {
        try
        {
            if (string.IsNullOrEmpty(CertifierCodename))
                throw new Exception("Please Select Certifier");
            if (BolAccount.CertificationRequests.Keys.Contains(CertifierCodename))
                throw new Exception("Certification has already been requested from this certifier.");
            
            IsLoading = true;
            
            await _bolService.RequestCertification(CertifierCodename, token);

            while (!BolAccount.CertificationRequests.ContainsKey(CertifierCodename))
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                await UpdateBolAccount(token);
            }

            CertifierCodename = string.Empty;

            await Toast.Make("Certification request sent successfully.").Show();
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
    private async Task PayCertificationFees(CancellationToken token)
    {
        try
        {
            if (!IsAccountPendingFees)
            {
                return;
            }
            
            IsLoading = true;
            
            await _bolService.PayCertificationFees(token);

            while (!IsAccountOpen)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                await UpdateBolAccount(token);
            }
            
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

        if (string.IsNullOrEmpty(userdata?.IdentificationMatrix) &&
            string.IsNullOrEmpty(userdata?.IdentificationMatrixCompany))
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

public class CertifierListItem
{
    public string CodeName { get; set; }
    public string Fee { get; set; }
    public Color Color { get; set; }
}
