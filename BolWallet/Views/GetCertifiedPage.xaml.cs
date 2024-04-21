using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;

public partial class GetCertifiedPage : ContentPage
{
    private CancellationTokenSource _cts;

    public GetCertifiedPage(GetCertifiedViewModel getCertifiedViewModel)
	{
		InitializeComponent();
		BindingContext = getCertifiedViewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
        _cts = new CancellationTokenSource();
		await ((GetCertifiedViewModel)BindingContext).Initialize(_cts.Token);
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

            ((GetCertifiedViewModel)BindingContext).CertifierCodename = label.Text;
			Toast.Make("Copied to Clipboard").Show();
		}
	}
}
