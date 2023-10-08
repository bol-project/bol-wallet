namespace BolWallet.Models;
public partial class MoveClaimForm : ObservableObject
{
	[ObservableProperty]
	public string _comAddress;

	[ObservableProperty]
	private decimal _amount;
}

