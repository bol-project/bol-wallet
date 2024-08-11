using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace BolWallet;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		Window.AddFlags(WindowManagerFlags.KeepScreenOn);

        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
	}
}
