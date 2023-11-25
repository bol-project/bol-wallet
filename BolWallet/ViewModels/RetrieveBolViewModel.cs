using CommunityToolkit.Maui.Alerts;

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

    public void OnClickCopyCodename()
    {
        Clipboard.Default.SetTextAsync("Codename");

        Toast.Make("Codename copied to Clipboard").Show();
    }

    public void OnClickCopyAddress()
    {
        Clipboard.Default.SetTextAsync("Address");

        Toast.Make("Address copied to Clipboard").Show();
    }
}
