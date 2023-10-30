using Bol.Core.Model;
using System.Text.RegularExpressions;

namespace BolWallet.Models;

public partial class BolTransactionEntryListItem : ObservableObject
{
    public BolTransactionEntry BolTransactionEntry { get; set; }

    private string UserCodeName { get; set; }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged();
        }
    }
    private bool IsRecievingTransaction => UserCodeName == BolTransactionEntry.ReceiverCodeName;
    public string BolAmount => (IsRecievingTransaction? "+" : "-") + BolTransactionEntry.Amount + " BOL";

    public Color AmountTextColor => IsRecievingTransaction ? Colors.LawnGreen : Colors.Red;  

    public BolTransactionEntryListItem(string userCodeName, BolTransactionEntry bolTransactionEntry)
    {
        UserCodeName = userCodeName;
        BolTransactionEntry = bolTransactionEntry;
        IsExpanded = false;
    }
}

