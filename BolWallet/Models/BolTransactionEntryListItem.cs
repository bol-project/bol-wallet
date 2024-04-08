using Bol.Core.Model;

namespace BolWallet.Models;

public partial class BolTransactionEntryListItem : ObservableObject
{
    public BolTransactionEntry BolTransactionEntry { get; set; }

    public string BolTransactionEntrySenderCodeName => string.IsNullOrEmpty(BolTransactionEntry.SenderCodeName) ? "-" : BolTransactionEntry.SenderCodeName;

    public string BolTransactionEntrySenderAddress => string.IsNullOrEmpty(BolTransactionEntry.SenderAddress) ? "-" : BolTransactionEntry.SenderAddress;

    public string BolTransactionEntryReceiverCodeName => string.IsNullOrEmpty(BolTransactionEntry.ReceiverCodeName) ? "-" : BolTransactionEntry.ReceiverCodeName;

    public string BolTransactionEntryReceiverAddress => string.IsNullOrEmpty(BolTransactionEntry.ReceiverAddress) ? "-" : BolTransactionEntry.ReceiverAddress;

    private string UserCodeName { get; set; }

	[ObservableProperty]
	private bool _isExpanded;
    
    public bool IsReceivingTransaction => UserCodeName == BolTransactionEntry.ReceiverCodeName;
    public string BolAmount => (IsReceivingTransaction ? "+" : "-") + BolTransactionEntry.Amount + " BOL";

    public string AmountTextColor => IsReceivingTransaction ? MudBlazor.Colors.Green.Default : MudBlazor.Colors.Red.Default;  

    public BolTransactionEntryListItem(string userCodeName, BolTransactionEntry bolTransactionEntry)
    {
        UserCodeName = userCodeName;
        BolTransactionEntry = bolTransactionEntry;
        IsExpanded = false;
    }
}

