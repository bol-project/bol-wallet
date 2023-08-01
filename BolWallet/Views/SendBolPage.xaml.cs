namespace BolWallet.Views;
public partial class SendBolPage : ContentPage
{
    public SendBolPage(SendBolViewModel sendBolViewModel)
    {
        InitializeComponent();
        BindingContext = sendBolViewModel;
    }
}