namespace BolWallet;

public partial class App : Application
{
	private readonly ICountriesService _countriesService;

	public App(MainPage mainPage, ICountriesService countriesService)
	{
		_countriesService = countriesService;
		InitializeComponent();
	
		MainPage = new NavigationPage(mainPage);
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		Window window = base.CreateWindow(activationState);

		window.Created += async (sender, args) => await _countriesService.GetAsync();

		return window;
	}
}
