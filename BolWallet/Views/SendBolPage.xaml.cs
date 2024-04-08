namespace BolWallet.Views;
public partial class SendBolPage : ContentPage
{
    public SendBolPage(SendBolViewModel sendBolViewModel)
    {
        InitializeComponent();
        BindingContext = sendBolViewModel;
    }

    private async void OnTapCommercialAddress(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            await ((SendBolViewModel)BindingContext).SelectCommercialAddressCommand.ExecuteAsync(label.Text);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((SendBolViewModel)BindingContext).Initialize();
    }
}