using BolApp.ViewModels;

namespace BolApp;

public partial class CodenamePage : ContentPage
{
	public CodenamePage(CodenameViewModel codenameViewModel)
	{
		InitializeComponent();
		BindingContext = codenameViewModel;
	}
}