namespace BolWallet.Views;
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AccountPage : ContentPage
{

    public AccountPage(AccountViewModel accountViewModel)
    {
        InitializeComponent();
        BindingContext = accountViewModel;
    }

}