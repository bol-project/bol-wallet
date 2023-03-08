using Bol.Core.Model;

namespace BolWallet.Views;

public partial class CreateEdiPage : ContentPage
{
	public CreateEdiPage(CreateEdiViewModel ediViewModel)
	{
		InitializeComponent();
		BindingContext = ediViewModel;
	}
}