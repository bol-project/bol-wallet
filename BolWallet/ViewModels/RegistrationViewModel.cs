using Bol.Core.Abstractions;
using Bol.Core.Model;

namespace BolWallet.ViewModels;
public partial class RegistrationViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public RegistrationViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService)
		: base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
	}

	[ObservableProperty]
	private string _codename = "";

	[ObservableProperty]
	private string _edi = "";

	[ObservableProperty]
	private string _mainAddress = "";

	[ObservableProperty]
	private bool _isLoading = false;

	public async Task Initialize()
	{
		var userData = await _secureRepository.GetAsync<UserData>("userdata");
		if (userData is null) return;

		Codename = userData.Codename;
		Edi = userData.Edi;
		MainAddress = userData.BolWallet.accounts.FirstOrDefault(a => a.Label == "main").Address;
	}

	[RelayCommand]
	private async Task Register()
	{
		IsLoading = true;

		BolAccount bolAccount = await _bolService.Register();

		IsLoading = false;
	}
}
