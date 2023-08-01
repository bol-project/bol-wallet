namespace BolWallet.ViewModels;
public partial class RetrieveBolViewModel : BaseViewModel
{
    private UserData _userData;

    public string RetrieveBolLabel => "Retrieve Bol";
    public string CodenameText => "My Codename";
    public string CommAddressText => "My Commercial Address";
    public string AmountText => "Amount";

    public RetrieveBolViewModel(INavigationService navigationService) : base(navigationService)
    {

    }
}
