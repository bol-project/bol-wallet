namespace BolWallet;
public partial class UserPage : ContentPage
{
    public UserPage(UserViewModel userViewModel)
    {
        InitializeComponent();
        BindingContext = userViewModel;
    }
}