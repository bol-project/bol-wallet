namespace BolWallet.Models;
public partial class MoveClaimForm : ObservableObject
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

