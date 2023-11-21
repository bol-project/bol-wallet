using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Newtonsoft.Json;
using System.Text;

namespace BolWallet.ViewModels;

public partial class AccountViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;
	private readonly IFileSaver _fileSaver;

	public AccountViewModel(
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
	private BolAccount _bolAccount;

	public async Task Initialize(CancellationToken cancellationToken = default)
	{
		try
		{
			userData = await _secureRepository.GetAsync<UserData>("userdata");

			BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));


		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
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