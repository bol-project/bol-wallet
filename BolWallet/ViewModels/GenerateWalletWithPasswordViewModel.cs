using Bol.Core.Abstractions;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace BolWallet.ViewModels;
public partial class GenerateWalletWithPasswordViewModel : BaseViewModel
{
	private readonly IWalletService _walletService;
	private readonly ISecureRepository _secureRepository;
	private readonly ISha256Hasher _sha256Hasher;
	private readonly IBase16Encoder _base16Encoder;
	private readonly IFileSaver _fileSaver;

	public GenerateWalletWithPasswordViewModel(
		INavigationService navigationService,
		IWalletService walletService,
		ISecureRepository secureRepository,
		ISha256Hasher sha256Hasher,
		IBase16Encoder base16Encoder,
		IFileSaver fileSaver) : base(navigationService)
	{
		_walletService = walletService;
		_secureRepository = secureRepository;
		_sha256Hasher = sha256Hasher;
		_base16Encoder = base16Encoder;
		_fileSaver = fileSaver;
	}

	[ObservableProperty]
	private string _password = "";


	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private string _passwordErrorMessage = "";

	[RelayCommand]
	private async Task Submit()
	{
		try
		{
			if (string.IsNullOrEmpty(Password))
			{
				await Toast.Make("Please enter a password.").Show();
				return;
			}

			if (!IsPasswordStrong(Password))
			{
				PasswordErrorMessage = "Password must be at least 8 characters long and contain a mix of uppercase, lowercase, numbers, and special characters.";
				return;
			}

			PasswordErrorMessage = ""; 
			IsLoading = true;

			byte[] hash = _sha256Hasher.Hash(Encoding.UTF8.GetBytes(Password));

			string privateKey = _base16Encoder.Encode(hash);

			UserData userData = await this._secureRepository.GetAsync<UserData>("userdata");

			var bolWallet = await Task.Run(() => _walletService.CreateWallet(Password, userData.Codename, userData.Edi, privateKey));

			userData.BolWallet = bolWallet;
			userData.WalletPassword = Password;

			await Task.Run(async () => await _secureRepository.SetAsync("userdata", userData));

			await Clipboard.SetTextAsync(System.Text.Json.JsonSerializer.Serialize(bolWallet));

			await DownloadWalletAsync(bolWallet);

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

	private bool IsPasswordStrong(string password)
	{
		const string strongPasswordPattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$";

		return Regex.IsMatch(password, strongPasswordPattern);
	}

	[RelayCommand]
	private async Task DownloadWalletAsync(Bol.Core.Model.BolWallet bolWallet,CancellationToken cancellationToken = default)
	{
		try
		{
			string json = JsonConvert.SerializeObject(bolWallet);

			byte[] jsonData = Encoding.UTF8.GetBytes(json);

			using (var stream = new MemoryStream(jsonData))
			{
				string fileName = "BolWallet.json";

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
}