using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System.Reflection;

namespace BolWallet.ViewModels;
public partial class MainWithAccountViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	//private readonly IBolService _bolService;

	public string AccountText => "Account";
	public string SendText => "Send";
	public string RecieveText => "Recieve";
	public string ClaimText => "Claim";
	public string MoveClaimText => "Move Claim";
	public string CommunityText => "Bol Community";

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private bool _isLoading = false;

	private UserData userData;

    public MainWithAccountViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository
        /*IBolService bolService*/) : base(navigationService)
    {
        userData = new UserData
        {
            Codename = "Codename Dummy",
            Edi = "Edi Dummy"
        };
        _secureRepository = secureRepository;
        //_bolService = bolService;
    }
    public async Task Initialize()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("BolWallet.bol_account.json");
            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync();
            _bolAccount = JsonConvert.DeserializeObject<BolAccount>(jsonContent);
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
			//BolAccount = await _bolService.Claim();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

	[RelayCommand]
	private void NavigateToUserPage()
	{
		NavigationService.NavigateTo<AccountViewModel>(true);
	}

	[RelayCommand]
	private void NavigateToBolCommunityPage()
	{
		NavigationService.NavigateTo<BolCommunityViewModel>(true);
	}

	[RelayCommand]
	private void NavigateToSendBolPage()
	{
		NavigationService.NavigateTo<SendBolViewModel>(true);
	}

	[RelayCommand]
	private void NavigateToMoveClaimPage()
	{
		NavigationService.NavigateTo<MoveClaimViewModel>(true);
	}

	[RelayCommand]
	private void NavigateToRetrieveBolPage()
	{
		NavigationService.NavigateTo<RetrieveBolViewModel>(true);
	}
}
