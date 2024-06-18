using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;

namespace BolWallet;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		Window.AddFlags(WindowManagerFlags.KeepScreenOn);

		AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
	}
}
