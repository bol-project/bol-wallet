namespace BolWallet.Models;
public partial class SendBolForm : ObservableObject
{
    private string _comAddress;
    public string ComAddress
    {
        get => _comAddress;
        set
        {
            _comAddress = value;
            OnPropertyChanged();
        }
    }

    private string _recieverCodename;
    public string RecieverCodename
    {
        get => _recieverCodename;
        set
        {
            _recieverCodename = value;
            OnPropertyChanged();
        }
    }

    private string _recieverAddress;
    public string RecieverAddress
    {
        get => _recieverAddress;
        set
        {
            _recieverAddress = value;
            OnPropertyChanged();
        }
    }

    private double _amount;
    public double Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            OnPropertyChanged();
        }
    }
}
