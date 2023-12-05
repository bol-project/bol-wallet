namespace BolWallet.Views;

public partial class GenerateWalletWithPasswordPage : ContentPage
{
	public GenerateWalletWithPasswordPage(GenerateWalletWithPasswordViewModel generateWalletWithPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = generateWalletWithPasswordViewModel;
	}
}