namespace BolWallet.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    public MainPageViewModel(INavigationService navigationService) : base(navigationService)
    {
    }

    [RelayCommand]
    private void NavigateToCodenamePage()
    {
        NavigationService.NavigateToPage<CodenamePage>(true);
    }
}