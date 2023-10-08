namespace BolWallet.ViewModels;

public class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

	protected UserData userData;

	protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}