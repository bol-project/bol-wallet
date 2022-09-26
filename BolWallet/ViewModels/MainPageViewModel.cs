using BolWallet.Services;

namespace BolWallet.ViewModels;

public class MainPageViewModel
{
	private readonly INavigationService _navigationService;

	public MainPageViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public Command NavigateToCodenamePage => new( () => _navigationService.NavigateToPage<CodenamePage>(true));
}