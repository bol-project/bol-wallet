using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System.Reflection;

namespace BolWallet.ViewModels;
public partial class MainWithAccountViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public string WelcomeText => "Welcome";
	public string BalanceText => "Total Balance";
    public string AccountText => "Account";
	public string SendText => "Send";
	public string RecieveText => "Recieve";
	public string ClaimText => "Claim";
	public string MoveClaimText => "Move Claim";
	public string CommunityText => "Bol Community";

	[ObservableProperty]
	private string _codeName = "";

	[ObservableProperty]
	private string _mainAddress = "";

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private bool _isCertified = false;

	[ObservableProperty]
	private bool _isRegistered = false;

	public MainWithAccountViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
	}

	public async Task Initialize()
	{
		await UpdateBolAccount();
	}

	[RelayCommand]
	private async Task UpdateBolAccount()
	{
		try
		{
			userData = await _secureRepository.GetAsync<UserData>("userdata");

			CodeName = userData.Codename;
			MainAddress = userData.BolWallet.accounts?.FirstOrDefault(a => a.Label == "main").Address;

			BolAccount = await _bolService.GetAccount(userData.Codename);

			IsRegistered = true;

			if (BolAccount.AccountStatus == AccountStatus.PendingFees || BolAccount.AccountStatus == AccountStatus.Open)
				IsCertified = true;
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

    [RelayCommand]
	private async Task Claim()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount = await _bolService.Claim();
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
	private async Task Register()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount = await _bolService.Register();

			await Toast.Make("Your Account is registered now.").Show();
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
	private async Task NavigateToCertifyPage()
	{
		if (IsRegistered)
			await NavigationService.NavigateTo<CertifyViewModel>(true);
		else
			await Toast.Make("CodeName is not a registered Bol Account.").Show();
	}

	[RelayCommand]
	private async Task NavigateToTransactionsPage()
	{
		await NavigationService.NavigateTo<TransactionsViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToBolCommunityPage()
	{
		await NavigationService.NavigateTo<BolCommunityViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToSendBolPage()
	{
		await NavigationService.NavigateTo<SendBolViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToMoveClaimPage()
	{
		await NavigationService.NavigateTo<MoveClaimViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToRetrieveBolPage()
	{
		await NavigationService.NavigateTo<RetrieveBolViewModel>(true);
	}
}
