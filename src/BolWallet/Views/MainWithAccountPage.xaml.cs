using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class MainWithAccountPage : ContentPage
{
    private readonly MainWithAccountViewModel _mainWithAccountViewModel;
    public MainWithAccountPage(MainWithAccountViewModel mainWithAccountViewModel)
    {
        _mainWithAccountViewModel = mainWithAccountViewModel;
        InitializeComponent();
        BindingContext = _mainWithAccountViewModel;

        Unloaded += (_, _) => _mainWithAccountViewModel?.Dispose();
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        await Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(10);
            _mainWithAccountViewModel.IsRefreshing = true;
        });
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
