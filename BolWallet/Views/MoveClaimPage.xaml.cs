namespace BolWallet.Views
{
	public partial class MoveClaimPage : ContentPage
	{
		public MoveClaimPage (MoveClaimViewModel moveClaimViewModel)
		{
			InitializeComponent ();
			BindingContext = moveClaimViewModel;
		}
	}
}