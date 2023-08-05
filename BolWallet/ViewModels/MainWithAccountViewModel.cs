namespace BolWallet.ViewModels;
public partial class MainWithAccountViewModel : BaseViewModel
{
    public string WelcomeText => "Welcome to Bol " + GetCodename + "!";

    public string AccountText => "Account";
    public string SendText => "Send";
    public string RecieveText => "Recieve";
    public string ClaimText => "Claim";
    public string MoveClaimText => "Move Claim";
    public string CommunityText => "Bol Community";

    public MainWithAccountViewModel(INavigationService navigationService) : base(navigationService)
    {
        //Mock User Creation
        UserData = new UserData
        {
            Codename = "Codename Dummy",
            Edi = "Edi Dummy"
        };
    }

    [RelayCommand]
    private void NavigateToUserPage()
    {
        NavigationService.NavigateTo<UserViewModel>(true);
    }

    [RelayCommand]
    private void NavigateToBolCommunityPage()
    {
        NavigationService.NavigateTo<BolCommunityViewModel>(true);
    }

    [RelayCommand]
    private void NavigateToSendBolPage()
    {
        NavigationService.NavigateTo<SendBolViewModel>(true);
    }

    [RelayCommand]
    private void NavigateToMoveClaimPage()
    {
        NavigationService.NavigateTo<MoveClaimViewModel>(true);
    }

    [RelayCommand]
    private void NavigateToRetrieveBolPage()
    {
        NavigationService.NavigateTo<RetrieveBolViewModel>(true);
    }
}
