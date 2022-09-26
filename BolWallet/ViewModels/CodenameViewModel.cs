using BolApp.Services;

namespace BolApp.ViewModels;

public class CodenameViewModel
{
	private readonly INavigationService _navigationService;

	public CodenameViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}
}