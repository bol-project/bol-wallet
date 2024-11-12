using BolWallet.Controls;
using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet;

public partial class App : Application, IRecipient<TargetNetworkChangedMessage>
{
    private readonly INetworkPreferences _networkPreferences;

    public App(INetworkPreferences networkPreferences, IMessenger messenger)
	{
        _networkPreferences = networkPreferences;
        InitializeComponent();

        messenger.Register(this);
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
    }
    
    protected override Window CreateWindow(IActivationState activationState)
    {
        var preloadPage = activationState.Context.Services.GetRequiredService<PreloadPage>();
        return new Window { Title = CreateWindowTitle(), Page = new NavigationPage(preloadPage) };
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
        MainThread.BeginInvokeOnMainThread(() => Current.Windows[0].Page.Window.Title = CreateWindowTitle());
    }

    private string CreateWindowTitle() => _networkPreferences.IsMainNet
        ? Constants.AppName
        : $"{Constants.AppName} ({_networkPreferences.Name})";
}
