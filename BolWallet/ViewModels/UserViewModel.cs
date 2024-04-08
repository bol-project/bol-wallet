namespace BolWallet.ViewModels;

public partial class UserViewModel : BaseViewModel
{
    private UserData _userData;

    public UserViewModel(INavigationService navigationService) : base(navigationService)
    {
        //Mock User Creation
        _userData = new UserData
        {
            Codename = "Codename Dummy",
            Edi = "Edi Dummy"
        };
    }

    [ObservableProperty]
    private IndividualCodenameForm _individualCodenameForm;

    public string CodenameLabel => "Greetings " + _userData?.Codename + "!";

    public string EdiLabel => "Edi: " + _userData?.Edi;
    public string Address { get; set; }
    // TODO section

    //Replace 2nd string with var
    public string BolLabel => "Bol: " + "Insert Bol Var here";

    //Replace button text and var name for both below

    public string TransactionButton1Label => "Transaction 1";
    public string TransactionButton2Label => "Transaction 2";

    //Implement the navigation commands after creation of their respective views

    [RelayCommand]
    private void NavigateToTransaction1Page()
    {
        //NavigationService.NavigateTo<>(true);
    }

    [RelayCommand]
    private void NavigateToTransaction2Page()
    {
        //NavigationService.NavigateTo<>(true);
    }

    [RelayCommand]
    private void NavigateToTSettingsPage()
    {
        //NavigationService.NavigateTo<>(true);
    }

}
