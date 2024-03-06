using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;

namespace BolWallet.Views;
public partial class TransactionsPage : ContentPage
{
    private readonly IOptions<BolWalletAppConfig> _bolConfig;

    public TransactionsPage(TransactionsViewModel transactionsViewModel,
         IOptions<BolWalletAppConfig> bolConfig)
    {
        InitializeComponent();
        BindingContext = transactionsViewModel;
        _bolConfig = bolConfig;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((TransactionsViewModel)BindingContext).Initialize();
	}

	private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if(e.SelectedItem != null)
        {
            BolTransactionEntryListItem transaction = (BolTransactionEntryListItem)e.SelectedItem;
            transaction.IsExpanded = !transaction.IsExpanded;
            ((ListView)sender).SelectedItem = null;
        }
    }

    private void OnOpenInBrowserClicked(object sender, EventArgs e)
    {
        try
        {
            var transactionHash = sender is Button button ? button.CommandParameter.ToString() : null;
            if(transactionHash != null)
            {
                Uri uri = new Uri($"{_bolConfig.Value.BolExplorerEndpoint}/transaction/" + transactionHash);
                Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                return;
            }
            Toast.Make("There was an error retrieving the transaction...").Show();
        }
        catch (Exception ex)
        {
            Toast.Make("ERROR: " + ex.Message).Show();
        }
    }

    private void OnCopyButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Clipboard.Default.SetTextAsync(button.CommandParameter.ToString());

            Toast.Make("Copied to Clipboard").Show();
        }
    }
}
