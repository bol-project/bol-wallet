using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
	private readonly IPermissionService _permissionService;
	private readonly ISecureRepository _secureRepository;

	public MainViewModel(
		INavigationService navigationService,
		IPermissionService permissionService,
		ISecureRepository secureRepository) : base(navigationService)
	{
		_permissionService = permissionService;
		_secureRepository = secureRepository;
	}

	[RelayCommand]
	private void NavigateToCodenamePage()
	{
		NavigationService.NavigateTo<CreateCodenameViewModel>(true);
	}


	[RelayCommand]
	private async Task ImportYourWallet()
	{
		if (await _permissionService.CheckPermissionAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
		{
			await _permissionService.DisplayWarningAsync<Permissions.StorageRead>();
			return;
		}

		try
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};

			var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
			{
			   { DevicePlatform.iOS, new[] { "public.json" } },
			   { DevicePlatform.Android, new[] { "application/json" } },
			   { DevicePlatform.macOS, new[] { "json" } },
			   { DevicePlatform.WinUI, new[] { ".json", "application/json" } }
			});

			var pickResult = await FilePicker.PickAsync(new PickOptions
			{
				FileTypes = customFileType,
				PickerTitle = "Import Your Json Wallet"
			});

			var jsonString = File.ReadAllText(pickResult.FullPath);

			var bolWallet = JsonSerializer.Deserialize<Bol.Core.Model.BolWallet>(jsonString, options);

			var password = await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayPromptAsync("Enter Your Password", null);

			var userData = new UserData
			{
				Codename = bolWallet.Name,
				BolWallet = bolWallet,
				WalletPassword = password
			};

			userData.IsRegisteredAccount = true;
			userData.AccountStatus = Bol.Core.Model.AccountStatus.Open;

			await _secureRepository.SetAsync("userdata", userData);

			await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

}