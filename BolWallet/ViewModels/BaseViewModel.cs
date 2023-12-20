using Blazing.Mvvm.ComponentModel;

namespace BolWallet.ViewModels;

public class BaseViewModel : ViewModelBase
{
    protected readonly INavigationService NavigationService;

	protected UserData userData;

	protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}