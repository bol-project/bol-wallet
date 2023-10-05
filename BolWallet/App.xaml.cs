﻿using BolWallet.Controls;
using SkiaSharp.Extended.Iconify;
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
		
#if WINDOWS
        Microsoft.Maui.Handlers.PickerHandler.Mapper.Add(nameof(View.HorizontalOptions), MapHorizontalOptions);
#endif
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
			
		SKTextRunLookup.Instance.AddFontAwesome();

		UserData userData = secureRepository.Get<UserData>("userdata");

		if (userData?.BolWallet == null)
		{
			MainPage = new NavigationPage(mainPage);
			return;
		}

		using var scope = serviceProvider.CreateScope();

		ContentPage contentPage = new ContentPage();

		if (!userData.IsRegisteredAccount)
		{
			contentPage = scope.ServiceProvider.GetRequiredService<RegistrationPage>();
			MainPage = new NavigationPage(contentPage);
			return;
		}

		if (userData.AccountStatus != Bol.Core.Model.AccountStatus.Open)
		{
			contentPage = scope.ServiceProvider.GetRequiredService<CertifyPage>();
			MainPage = new NavigationPage(contentPage);
			return;
		}

		contentPage = scope.ServiceProvider.GetRequiredService<MainWithAccountPage>();
		MainPage = new NavigationPage(contentPage);
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		Window window = base.CreateWindow(activationState);

		window.Created += async (sender, args) => await _countriesService.GetAsync();

		return window;
	}

#if WINDOWS
    private static void MapHorizontalOptions(IViewHandler handler, IView view)
    {
	    if (view is not View mauiView)
	    {
		    return;
	    }

	    if (handler.PlatformView is not Microsoft.UI.Xaml.FrameworkElement element)
	    {
		    return;
	    }

	    element.HorizontalAlignment = mauiView.HorizontalOptions.Alignment switch
	    {
		    LayoutAlignment.Start  => Microsoft.UI.Xaml.HorizontalAlignment.Left,
		    LayoutAlignment.Center => Microsoft.UI.Xaml.HorizontalAlignment.Center,
		    LayoutAlignment.End    => Microsoft.UI.Xaml.HorizontalAlignment.Right,
		    LayoutAlignment.Fill   => Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
		    _ => throw new ArgumentOutOfRangeException()
	    };
    }

#endif
}
