namespace BolWallet.Views;

public partial class ClaimPage : ContentPage
{
	public ClaimPage(ClaimViewModel claimViewModel)
	{
		InitializeComponent();
		BindingContext = claimViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((ClaimViewModel)BindingContext).Initialize();
	}
}