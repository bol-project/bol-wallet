namespace BolWallet.Views;

public partial class WhitelistPage : ContentPage
{
	public WhitelistPage(WhitelistViewModel whitelistViewModel)
	{
		InitializeComponent();
		BindingContext = whitelistViewModel;
	}
}
