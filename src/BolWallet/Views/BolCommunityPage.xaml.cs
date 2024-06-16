using CommunityToolkit.Maui.Alerts;

namespace BolWallet.Views;
public partial class BolCommunityPage : ContentPage
{
    public BolCommunityPage(BolCommunityViewModel bolCommunityViewModel)
    {
        InitializeComponent();
        BindingContext = bolCommunityViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((BolCommunityViewModel)BindingContext).Initialize();
    }
}
