namespace BolWallet.Views;
public partial class TransactionsPage : ContentPage
{
    public TransactionsPage(TransactionsViewModel transactionsViewModel)
    {
        InitializeComponent();
        BindingContext = transactionsViewModel;
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
}