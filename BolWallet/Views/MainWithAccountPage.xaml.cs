using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class MainWithAccountPage : ContentPage
{
    public MainWithAccountPage(MainWithAccountViewModel mainWithAccountViewModel)
    {
        InitializeComponent();
        BindingContext = mainWithAccountViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((MainWithAccountViewModel)BindingContext).Initialize();
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