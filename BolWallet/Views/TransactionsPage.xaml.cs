namespace BolWallet.Views;
public partial class TransactionsPage : ContentPage
{
    public TransactionsPage(TransactionsViewModel transactionsViewModel)
    {
        InitializeComponent();
        BindingContext = transactionsViewModel;
    }
}