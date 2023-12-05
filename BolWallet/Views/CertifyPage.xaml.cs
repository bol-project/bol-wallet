using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;

public partial class CertifyPage : ContentPage
{
	public CertifyPage(CertifyViewModel certifyViewModel)
	{
		InitializeComponent();
		BindingContext = certifyViewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((CertifyViewModel)BindingContext).Initialize();
	}

	private void OnTapCopy(object sender, EventArgs e)
	{
		if (sender is Label label)
		{
			Clipboard.Default.SetTextAsync(label.Text);

			Toast.Make("Copied to Clipboard").Show();
		}
	}
}