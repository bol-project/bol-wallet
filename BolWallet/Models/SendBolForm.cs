using System.Numerics;

namespace BolWallet.Models;

public partial class SendBolForm : ObservableObject
{
	[ObservableProperty]
	public string _comAddress;

	[ObservableProperty]
	public string _receiverCodename;

	[ObservableProperty]
	public string _receiverAddress;

	[ObservableProperty]
	private decimal _amount;
}
