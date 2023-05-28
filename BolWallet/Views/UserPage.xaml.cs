namespace BolWallet.Views;
public partial class UserPage : ContentPage
{
    public UserPage(UserViewModel userViewModel)
    {
        InitializeComponent();
        BindingContext = userViewModel;
    }
}