using BolWallet.Controls;

namespace BolWallet;

public partial class App : Application
{
	private readonly ICountriesService _countriesService;
	private readonly ISecureRepository _secureRepository;

	public App(MainPage mainPage, IServiceProvider serviceProvider, ISecureRepository secureRepository, ICountriesService countriesService)
	{
		_countriesService = countriesService;
		_secureRepository = secureRepository;

		InitializeComponent();

		UserAppTheme = AppTheme.Light;

		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(ExtendedTextEdit),
			(handler, view) =>
			{
#if __ANDROID__
				handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__
				handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
				handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
			});

		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(ExtendedComboBox),
			(handler, view) =>
			{
#if __ANDROID__
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__
				handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
				handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
			});
		UserData userData = null;
		Task.Run(async () => userData = await secureRepository.GetAsync<UserData>("userdata")).Wait();

		if (userData?.BolWallet == null)
		{
			MainPage = new NavigationPage(mainPage);
			return;
		}

		ContentPage contentPage = new ContentPage();

		if (!userData.IsRegisteredAccount)
		{
			contentPage = serviceProvider.GetRequiredService<RegistrationPage>();
			MainPage = new NavigationPage(contentPage);
			return;
		}

		if (userData.AccountStatus != Bol.Core.Model.AccountStatus.Open)
		{
			contentPage = serviceProvider.GetRequiredService<CertifyPage>();
		}

		MainPage = new NavigationPage(contentPage);
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		Window window = base.CreateWindow(activationState);

		window.Created += async (sender, args) => await _countriesService.GetAsync();

		return window;
	}
}
