using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class RetrieveBolPage : ContentPage
{
    public RetrieveBolPage(RetrieveBolViewModel retrieveBolViewModel)
    {
        InitializeComponent();
        BindingContext = retrieveBolViewModel;
    }

    private void OnClickCopyCodename(object sender, EventArgs e)
    {
        Clipboard.Default.SetTextAsync("Codename");

        Toast.Make("Codename copied to Clipboard").Show();
    }

    private void OnClickCopyAddress(object sender, EventArgs e)
    {
        Clipboard.Default.SetTextAsync("Address");

        Toast.Make("Address copied to Clipboard").Show();
    }
}