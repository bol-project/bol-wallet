namespace BolWallet.ViewModels;

public class BaseViewModel
{
    protected readonly INavigationService NavigationService;

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}