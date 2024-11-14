namespace BolWallet;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
		BindingContext = mainViewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        return false;
    }
}
