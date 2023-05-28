namespace BolWallet.ViewModels;

public partial class UserViewModel : BaseViewModel
{
    private UserData _userData;

    protected UserViewModel(INavigationService navigationService) : base(navigationService)
    {
        //Mock User Creation
        _userData.Codename = "Codename Dummy";
        _userData.Edi = "Edi Dummy";
    }

    public string CodenameLabel => "Codename: " + _userData?.Codename;

    public string EdiLabel => "Edi: " + _userData?.Edi;


}
