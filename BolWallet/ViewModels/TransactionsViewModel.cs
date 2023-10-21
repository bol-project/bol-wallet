using Bol.Core.Model;
using CommunityToolkit.Maui.Core.Extensions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Transactions;

namespace BolWallet.ViewModels;

public partial class TransactionsViewModel : BaseViewModel
{
    private BolAccount _bolAccount;


    public TransactionsViewModel(INavigationService navigationService) : base(navigationService)
    {
        Transactions = new();
        InitAsync();
    }

    public string TransactionsLabel => "Transactions";

    public string SenderCodeNameLabel => "Sender CodeName: ";
    public string SenderAddressLabel => "Sender Address: ";
    public string RecieverCodenameLabel => "Reciever CodeName: ";
    public string ReceiverAddressLabel => "Reciever Address: ";
    public string AmountLabel => "Amount: ";

    private List<BolTransactionEntryListItem> _transactions;
    public List<BolTransactionEntryListItem> Transactions
    {
        get => _transactions;
        set
        {
            _transactions = value;
            OnPropertyChanged();
        }
    }

    public string EdiLabel => "Edi: " + _bolAccount?.Edi;
    public string Address { get; set; }
    // TODO section

    //Replace 2nd string with var
    public string BolLabel => "Bol: " + _bolAccount?.TotalBalance;

    //Replace button text and var name for both below

    public string TransactionsButtonLabel => "Transactions";

    //Implement the navigation commands after creation of their respective views

    [RelayCommand]
    private void NavigateToTransactionsPage()
    {
        NavigationService.NavigateTo<TransactionsViewModel>(true);
    }

    [RelayCommand]
    private void HideAllTransactionDetails()
    {
        foreach (var transaction in Transactions) 
        {
            transaction.IsExpanded = false;
        }
    }

    private async Task InitAsync()
    {
        string filePath = "TestAccount.json";
        var stream = await FileSystem.OpenAppPackageFileAsync(filePath);

        if (stream != null)
        {
            string str = (new System.IO.StreamReader(stream)).ReadToEnd();
            _bolAccount = JsonConvert.DeserializeObject<BolAccount>(str);
            var userCodename = _bolAccount.CodeName;
            foreach(BolTransactionEntry transaction in _bolAccount.Transactions.Values)
            {
                Transactions.Add(new BolTransactionEntryListItem(userCodename, transaction));
            }
        }
    }

}
