using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;

public partial class CertifyPage : ContentPage
{
    private CancellationTokenSource _cts;

    public CertifyPage(CertifyViewModel certifyViewModel)
	{
		InitializeComponent();
		BindingContext = certifyViewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
        _cts = new CancellationTokenSource();
		await ((CertifyViewModel)BindingContext).Initialize(_cts.Token);
	}

    protected override void OnDisappearing()
    {
        _cts.Cancel();
        base.OnDisappearing();
    }

    private void OnTapCopy(object sender, EventArgs e)
	{
		if (sender is Label label)
		{
			Clipboard.Default.SetTextAsync(label.Text);

            ((CertifyViewModel)BindingContext).CertifierCodename = label.Text;
			Toast.Make("Copied to Clipboard").Show();
		}
	}
}
