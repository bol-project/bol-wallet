using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class ClaimViewModel : BaseViewModel
{
	private readonly IBolService _bolService;
	private readonly ISecureRepository _secureRepository;

	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	public ClaimViewModel(
		INavigationService navigationService,
		IBolService bolService,
		ISecureRepository secureRepository) : base(navigationService)
	{
		_bolService = bolService;
		_secureRepository = secureRepository;
	}

	public async Task Initialize()
	{
		try
		{
			userData = await _secureRepository.GetAsync<UserData>("userdata");

			if (userData is null) return;

			BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));
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
}
