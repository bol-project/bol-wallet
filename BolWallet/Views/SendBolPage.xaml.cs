namespace BolWallet.Views;
public partial class SendBolPage : ContentPage
{
    public SendBolPage(SendBolViewModel sendBolViewModel)
    {
        InitializeComponent();
        BindingContext = sendBolViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((SendBolViewModel)BindingContext).Initialize();
	}
}