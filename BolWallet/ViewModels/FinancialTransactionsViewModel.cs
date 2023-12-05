namespace BolWallet.ViewModels
{
	public partial class FinancialTransactionsViewModel : BaseViewModel
	{
		public FinancialTransactionsViewModel(INavigationService navigationService) : base(navigationService) { }

		[RelayCommand]
		private async Task NavigateToTransferPage()
		{
			await NavigationService.NavigateTo<SendBolViewModel>(true);
		}

		[RelayCommand]
		private async Task NavigateToTransferClaimPage()
		{
			await NavigationService.NavigateTo<MoveClaimViewModel>(true);
		}

		[RelayCommand]
		private async Task Claim()
		{
			await NavigationService.NavigateTo<ClaimViewModel>(true);
		}
	}
}
