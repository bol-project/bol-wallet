namespace BolWallet.Views;

public partial class PreloadPage : ContentPage
{
    public PreloadPage(PreloadViewModel preloadViewModel)
    {
        BindingContext = preloadViewModel;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((PreloadViewModel)BindingContext).OnInitializedAsync();
    }
}

