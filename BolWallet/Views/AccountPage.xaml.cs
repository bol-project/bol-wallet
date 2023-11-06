using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;

public partial class AccountPage : ContentPage
{

	public AccountPage(AccountViewModel accountViewModel)
	{
		InitializeComponent();
		BindingContext = accountViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((AccountViewModel)BindingContext).Initialize();
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
