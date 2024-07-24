namespace BolWallet.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        
        // Hide the navigation bar in all platforms except iOS.
        if (DeviceInfo.Platform != DevicePlatform.iOS)
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }
        // Hiding the navigation bar in iOS will push content in the menu bar/notch area,
        // but hiding only the back button, will still render the title bar under the notch.
        else
        {
            NavigationPage.SetHasBackButton(this, false);
        }
    }
}

