namespace BolWallet.ViewModels;
public partial class RetrieveBolViewModel : BaseViewModel
{
    public string RetrieveBolLabel => "Retrieve Bol";
    public string CodenameText => "My Codename";
    public string CommAddressText => "My Commercial Address";
    public string AmountText => "Amount";

    public RetrieveBolViewModel(INavigationService navigationService) : base(navigationService)
    {

    }
}
