using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class MainWithAccountPage : ContentPage
{
    public MainWithAccountPage(MainWithAccountViewModel mainWithAccountViewModel)
    {
        InitializeComponent();
        BindingContext = mainWithAccountViewModel;
    }

    private void OnClickTodo(object sender, EventArgs e)
    {
        Toast.Make("TODO").Show();
    }
}