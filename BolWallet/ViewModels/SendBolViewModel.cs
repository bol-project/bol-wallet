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
        TestLabel = "From Address: " + SendBolForm.ComAddress +
                    "\nReciever Codename: " + SendBolForm.RecieverCodename +
                    "\nReciever Address: " + SendBolForm.RecieverAddress +
                    "\nAmount: " + SendBolForm.Amount;
    }
}
