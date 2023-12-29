using BolWallet.Pages;

namespace BolWallet.ViewModels;
public partial class BolCommunityViewModel : BaseViewModel
{
    public string BolCommunityHeaderText => "Bol Community";
    public string CertifyText => "Certify";
    public string DeleteFakeAccountText => "Delete Face Account";
    public string DeleteExpiredAccountText => "Delete Expired Account";
    public string VoteText => "Vote";
    public string SignMessageText => "Sign Message";
    public string BolIdentityText => "Bol Identity";

    public BolCommunityViewModel(INavigationService navigationService) : base(navigationService)
    {

    }

    [RelayCommand]
    private async Task NavigateToGetCertificationsPage()
    {
        await NavigationService.NavigateTo<CertifyViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToWhitelistAndCertifyPage()
    {
        await NavigationService.NavigateTo<CertifierViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsCertifierPage()
    {
        await App.Current.MainPage.Navigation.PushAsync(new Views.RegisterAsCertifierPage());
    }
}
