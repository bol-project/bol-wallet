namespace BolWallet.Views
{
	public partial class MoveClaimPage : ContentPage
	{
		public MoveClaimPage (MoveClaimViewModel moveClaimViewModel)
		{
			InitializeComponent ();
			BindingContext = moveClaimViewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await ((MoveClaimViewModel)BindingContext).Initialize();
		}
	}
}