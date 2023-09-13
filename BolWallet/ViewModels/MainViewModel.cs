namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
	public MainViewModel(INavigationService navigationService) : base(navigationService)
	{
	}

	[RelayCommand]
	private void NavigateToCodenamePage()
	{
		NavigationService.NavigateTo<CreateCodenameViewModel>(true);
	}
}