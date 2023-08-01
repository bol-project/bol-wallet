namespace BolWallet.ViewModels;
public partial class SendBolViewModel : BaseViewModel
{
    private UserData _userData;

    public string SendBolLabel=> "Send Bol";

    public string FromAddressText => "From Com. Address:";
    public string RecieverCodenameText => "Reciever Codename:";
    public string RecieverAddressText => "Reciever Address:";
    public string AmountText => "";
    public string SendText => "Send";

    [ObservableProperty]
    public string _testLabel;

    [ObservableProperty]
    private SendBolForm _sendBolForm;

    public SendBolViewModel(INavigationService navigationService) : base(navigationService)
    {
        SendBolForm = new SendBolForm();
    }

    [RelayCommand]
    private void SendBol()
    {
        TestLabel = "From Address: " + _sendBolForm.ComAddress +
                    "\nReciever Codename: " + _sendBolForm.RecieverCodename +
                    "\nReciever Address: " + _sendBolForm.RecieverAddress +
                    "\nAmount: " + _sendBolForm.Amount;
    }
}
