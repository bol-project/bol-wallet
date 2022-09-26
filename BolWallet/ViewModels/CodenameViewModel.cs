using BolWallet.Services;

namespace BolWallet.ViewModels;

public class CodenameViewModel
{
	private readonly INavigationService _navigationService;

	public CodenameViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}
}