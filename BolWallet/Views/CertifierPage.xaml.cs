namespace BolWallet.Views;

public partial class CertifierPage : ContentPage
{
	public CertifierPage(CertifierViewModel certifierViewModel)
	{
		InitializeComponent();
		BindingContext = certifierViewModel;
	}
}