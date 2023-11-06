using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;

namespace BolWallet.ViewModels;

public partial class AccountViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public AccountViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
	}

	[ObservableProperty]
	private BolAccount _bolAccount;

	public async Task Initialize()
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

}