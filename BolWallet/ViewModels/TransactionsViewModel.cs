using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Services;
using CommunityToolkit.Maui.Core.Extensions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Transactions;

namespace BolWallet.ViewModels;

public partial class TransactionsViewModel : BaseViewModel
{
	[ObservableProperty]
	private BolAccount _bolAccount = new();

	private readonly IBolService _bolService;
	private readonly ISecureRepository _secureRepository;

	public string TransactionsLabel => "Transactions";

	[ObservableProperty]
	private ObservableCollection<BolTransactionEntryListItem> _transactions = new();

	public TransactionsViewModel(
		INavigationService navigationService,
		IBolService bolService,
		ISecureRepository secureRepository) : base(navigationService)
	{
		_bolService = bolService;
		_secureRepository = secureRepository;
	}

	[RelayCommand]
	private void HideAllTransactionDetails()
	{
		foreach (var transaction in Transactions)
		{
			transaction.IsExpanded = false;
		}
	}

	public async Task Initialize()
    {
		userData = await _secureRepository.GetAsync<UserData>("userdata");

		BolAccount = await _bolService.GetAccount(userData.Codename);

		foreach (BolTransactionEntry transaction in BolAccount.Transactions.Reverse().Select(x=>x.Value))
        {
            Transactions.Add(new BolTransactionEntryListItem(userData.Codename, transaction));
        }
    }
}
