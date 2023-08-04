namespace BolWallet.Views;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationViewModel registrationViewModel)
	{
		InitializeComponent();
		BindingContext = registrationViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await ((RegistrationViewModel)BindingContext).Initialize();
	}
}