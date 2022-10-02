namespace BolWallet.Views;

public partial class CodenamePage : ContentPage
{
	public CodenamePage(CodenameViewModel codenameViewModel)
	{
		InitializeComponent();
		BindingContext = codenameViewModel;
	}
}