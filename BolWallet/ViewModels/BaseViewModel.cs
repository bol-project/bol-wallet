namespace BolWallet.ViewModels;

public class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}