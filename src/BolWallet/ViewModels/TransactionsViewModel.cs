using Bol.Core.Abstractions;
using Bol.Core.Model;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;

namespace BolWallet.ViewModels;

public partial class TransactionsViewModel : BaseViewModel
{
	[ObservableProperty]
	private BolAccount _bolAccount = new();

	private readonly IBolService _bolService;
	private readonly ISecureRepository _secureRepository;
    private readonly INetworkPreferences _networkPreferences;

	public string TransactionsLabel => "Transactions";
    
	[ObservableProperty]
	private ObservableCollection<BolTransactionEntryListItem> _transactions = new();
    
    public TransactionsViewModel(
		INavigationService navigationService,
		IBolService bolService,
		ISecureRepository secureRepository,
        INetworkPreferences networkPreferences) : base(navigationService)
	{
		_bolService = bolService;
		_secureRepository = secureRepository;
        _networkPreferences = networkPreferences;
    }

	[RelayCommand]
	private void HideAllTransactionDetails()
	{
		foreach (var transaction in Transactions)
		{
			transaction.IsExpanded = false;
		}
	}
    
    [RelayCommand]
    private async Task OpenBrowserWithTransactionDetails(string transactionHash, CancellationToken token = default)
    {
        try
        {
            await Browser.OpenAsync(
                $"{_networkPreferences.TargetNetworkConfig.BolExplorerEndpoint}/transaction/{transactionHash}",
                BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await Toast.Make($"ERROR: {ex.Message}").Show(token);
        }
    }
    
    [RelayCommand]
    private async Task CopyTransactionHashToClipboard(string text, CancellationToken token = default)
    {
        await Clipboard.SetTextAsync(text);
        await Toast.Make("Copied to Clipboard").Show(token);
    }

	public async Task Initialize()
    {
		userData = await _secureRepository.GetAsync<UserData>("userdata");

		BolAccount = await _bolService.GetAccount(userData.Codename);

		foreach (BolTransactionEntry transaction in BolAccount.Transactions.OrderByDescending(x=>int.Parse(x.Key)).Select(x=>x.Value))
        {
            Transactions.Add(new BolTransactionEntryListItem(userData.Codename, transaction));
        }
    }
}
