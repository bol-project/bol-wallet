namespace BolWallet.Views;

public partial class FinancialTransactionsPage : ContentPage
{
	public FinancialTransactionsPage(FinancialTransactionsViewModel financialTransactionsViewModel)
	{
		InitializeComponent();
		BindingContext = financialTransactionsViewModel;
	}
}