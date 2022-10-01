namespace BolWallet.ViewModels;

public class MainPageViewModel : BaseViewModel
{
	public MainPageViewModel(INavigationService navigationService) : base(navigationService)
	{
	}

	public Command NavigateToCodenamePage => new(() => NavigationService.NavigateToPage<CodenamePage>(true));
}