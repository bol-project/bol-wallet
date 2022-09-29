using BolWallet.ViewModels;

namespace BolWallet;

public partial class CodenamePage : ContentPage
{
	public CodenamePage(CodenameViewModel codenameViewModel)
	{
		InitializeComponent();
		BindingContext = codenameViewModel;
	}
}