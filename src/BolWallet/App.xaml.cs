using BolWallet.Controls;
using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet;

public partial class App : Application, IRecipient<TargetNetworkChangedMessage>
{
    private readonly INetworkPreferences _networkPreferences;

    public App(PreloadPage preloadPage, INetworkPreferences networkPreferences)
	{
        _networkPreferences = networkPreferences;
        InitializeComponent();

        WeakReferenceMessenger.Default.Register(this);
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

        MainPage = new NavigationPage(preloadPage);
    }
    
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);
        window.Title = CreateWindowTitle();
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
    public void Receive(TargetNetworkChangedMessage message)
    {
        Current.MainPage.Window.Title = CreateWindowTitle();
    }

    private string CreateWindowTitle() => _networkPreferences.IsMainNet
        ? Constants.AppName
        : $"{Constants.AppName} ({_networkPreferences.Name})";
}
