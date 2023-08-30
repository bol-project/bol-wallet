namespace BolWallet.Views;
using CommunityToolkit.Maui.Alerts;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationViewModel registrationViewModel)
	{
		InitializeComponent();
		BindingContext = registrationViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((RegistrationViewModel)BindingContext).Initialize();
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