using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class BolCommunityView : ContentPage
{
    public BolCommunityView(BolCommunityViewModel bolCommunityViewModel)
    {
        InitializeComponent();
        BindingContext = bolCommunityViewModel;
    }

    private void OnClickTodo(object sender, EventArgs e)
    {
        Toast.Make("TODO").Show();
    }
}