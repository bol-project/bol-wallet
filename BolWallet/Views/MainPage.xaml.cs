namespace BolWallet;

public partial class MainPage : ContentPage
{
	RadialControlViewModel RadialVM = new();

    public MainPage(MainViewModel mainViewModel)
	{
		InitializeComponent();
		BindingContext = mainViewModel;
	}
}

