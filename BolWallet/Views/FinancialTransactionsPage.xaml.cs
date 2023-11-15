namespace BolWallet.Views;

public partial class FinancialTransactionsPage : ContentPage
{
	public FinancialTransactionsPage(FinancialTransactionsViewModel financialTransactionsViewModel)
	{
		InitializeComponent();
		BindingContext = financialTransactionsViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((FinancialTransactionsViewModel)BindingContext).Initialize();
	}
}