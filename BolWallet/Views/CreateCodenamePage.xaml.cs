namespace BolWallet.Views;

public partial class CreateCodenamePage : ContentPage
{
	public CreateCodenamePage(CreateCodenameViewModel createCodenameViewModel)
	{
		InitializeComponent();
		BindingContext = createCodenameViewModel;
	}
}