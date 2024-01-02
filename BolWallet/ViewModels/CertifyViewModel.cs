using Bol.Core.Abstractions;
using Bol.Core.Model;
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
    private readonly IFileSaver _fileSaver;

    public CertifyViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService,
        IFileSaver fileSaver) : base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
        _fileSaver = fileSaver;
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

			userData.AccountStatus = BolAccount.AccountStatus;

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
        var ediFiles = await this._secureRepository.GetAsync<EdiFiles>("ediFiles");

        if (ediFiles is null)
            return;

        var ediZipFiles = await CreateZipFile(ediFiles);

        using (var stream = new MemoryStream(ediZipFiles))
        {
            var result = await _fileSaver.SaveAsync("edi-images.zip", stream, cancellationToken);

            if (result.IsSuccessful)
            {
                await Toast.Make($"File 'edi-images.zip' saved successfully!").Show();
            }
        }
    }

    private async Task<byte[]> CreateZipFile(EdiFiles ediFiles, CancellationToken cancellationToken = default)
    {


        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (PropertyInfo property in ediFiles.GetType().GetProperties())
                {
                    var ediFileItem = property.GetValue(ediFiles) as EdiFileItem;
                    if (ediFileItem?.Content != null)
                    {
                        var zipEntry = archive.CreateEntry(ediFileItem.FileName, CompressionLevel.Optimal);
                        using (var entryStream = zipEntry.Open())
                            await entryStream.WriteAsync(ediFileItem.Content, 0, ediFileItem.Content.Length);
                    }
                }
            }

            return memoryStream.ToArray();
        }
    }

    private async Task DownloadDataAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] jsonData = Encoding.UTF8.GetBytes(json);

            using (var stream = new MemoryStream(jsonData))
            {
                var result = await _fileSaver.SaveAsync(fileName, stream, cancellationToken);

                if (result.IsSuccessful)
                {
                    await Toast.Make($"File '{fileName}' saved successfully!").Show();
                }
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task DownloadAccountAsync(CancellationToken cancellationToken = default)
    {
        await DownloadDataAsync(BolAccount, "BolAccount.json", cancellationToken);
    }

    [RelayCommand]
    private async Task DownloadBolWalletAsync(CancellationToken cancellationToken = default)
    {
        await DownloadDataAsync(userData.BolWallet, "BolWallet.json", cancellationToken);
    }
}
