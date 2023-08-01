namespace BolWallet.ViewModels;

public class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

    protected UserData UserData;

    public string GetCodename => UserData?.Codename;

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}