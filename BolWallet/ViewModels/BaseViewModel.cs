using Blazing.Mvvm.ComponentModel;

namespace BolWallet.ViewModels;

public partial  class BaseViewModel : ViewModelBase
{
    
    protected readonly INavigationService NavigationService;

	public UserData userData;

	protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}