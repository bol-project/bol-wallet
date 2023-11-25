using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class RegistrationViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public RegistrationViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
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

	public UserData userData;

	public async Task Initialize()
	{
		userData = await _secureRepository.GetAsync<UserData>("userdata");

		if (userData is null) return;

		Codename = userData.Codename;
		Edi = userData.Edi;
		MainAddress = userData.BolWallet.accounts?.FirstOrDefault(a => a.Label == "main").Address;
	}

	[RelayCommand]
	public async Task Register()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount bolAccount = await _bolService.Register();

			userData.IsRegisteredAccount = true;

			await Task.Run(async () => await _secureRepository.SetAsync("userdata", userData));

			await Task.Delay(1500);

			await NavigationService.NavigateTo<CertifyViewModel>(true);
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
