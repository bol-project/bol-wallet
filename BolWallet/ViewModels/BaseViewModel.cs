namespace BolWallet.ViewModels;

public class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

	public UserData userData;

	protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}